using System;
using NUnit.Framework;

namespace Deucarian.Monetization.Tests
{
    public sealed class MonetizationTests
    {
        private static readonly MonetizationPlacementId RewardedPlacement = new MonetizationPlacementId("rewarded.test");
        private static readonly MonetizationPlacementId InterstitialPlacement = new MonetizationPlacementId("interstitial.test");
        private static readonly MonetizationPlacementId AlternateInterstitialPlacement = new MonetizationPlacementId("interstitial.test.alternate");

        [Test]
        public void RewardedSuccessRecordsClaim()
        {
            MonetizationSession session = CreateSession();

            MonetizationResult result = session.ShowRewarded(
                RewardedPlacement,
                new RewardClaimId("claim.success"),
                Context(nowSeconds: 10, inCombat: false, terminalRuns: 1));

            Assert.AreEqual(MonetizationResultCode.Success, result.Code);
            Assert.IsTrue(result.Succeeded);
        }

        [Test]
        public void RewardedUnavailableDoesNotSucceed()
        {
            var provider = new MockMonetizationProvider();
            provider.SetAvailability(RewardedPlacement, MonetizationResultCode.Unavailable);
            MonetizationSession session = CreateSession(provider);

            MonetizationResult result = session.ShowRewarded(
                RewardedPlacement,
                new RewardClaimId("claim.unavailable"),
                Context(nowSeconds: 10, inCombat: false, terminalRuns: 1));

            Assert.AreEqual(MonetizationResultCode.Unavailable, result.Code);
        }

        [Test]
        public void RewardedCancelledDoesNotSucceed()
        {
            var provider = new MockMonetizationProvider();
            provider.EnqueueShowResult(RewardedPlacement, MonetizationResultCode.Cancelled);
            MonetizationSession session = CreateSession(provider);

            MonetizationResult result = session.ShowRewarded(
                RewardedPlacement,
                new RewardClaimId("claim.cancelled"),
                Context(nowSeconds: 10, inCombat: false, terminalRuns: 1));

            Assert.AreEqual(MonetizationResultCode.Cancelled, result.Code);
            Assert.IsFalse(result.Succeeded);
        }

        [Test]
        public void InterstitialBlockedDuringCombat()
        {
            MonetizationSession session = CreateSession();

            MonetizationResult result = session.ShowInterstitial(
                InterstitialPlacement,
                Context(nowSeconds: 10, inCombat: true, terminalRuns: 1));

            Assert.AreEqual(MonetizationResultCode.BlockedDuringCombat, result.Code);
        }

        [Test]
        public void InterstitialBlockedBeforeFirstRun()
        {
            MonetizationSession session = CreateSession();

            MonetizationResult result = session.ShowInterstitial(
                InterstitialPlacement,
                Context(nowSeconds: 10, inCombat: false, terminalRuns: 0));

            Assert.AreEqual(MonetizationResultCode.BlockedBeforeFirstRun, result.Code);
        }

        [Test]
        public void InterstitialCooldownBlocksImmediateRepeat()
        {
            MonetizationSession session = CreateSession();

            MonetizationResult first = session.ShowInterstitial(
                InterstitialPlacement,
                Context(nowSeconds: 10, inCombat: false, terminalRuns: 1));
            MonetizationResult second = session.ShowInterstitial(
                InterstitialPlacement,
                Context(nowSeconds: 20, inCombat: false, terminalRuns: 1));

            Assert.AreEqual(MonetizationResultCode.Success, first.Code);
            Assert.AreEqual(MonetizationResultCode.CooldownActive, second.Code);
        }

        [Test]
        public void SharedCooldownGroupBlocksSeparateInterstitialPlacements()
        {
            MonetizationSession session = CreateSession();

            MonetizationResult first = session.ShowInterstitial(
                InterstitialPlacement,
                Context(nowSeconds: 10, inCombat: false, terminalRuns: 1));
            MonetizationResult second = session.ShowInterstitial(
                AlternateInterstitialPlacement,
                Context(nowSeconds: 20, inCombat: false, terminalRuns: 1));

            Assert.AreEqual(MonetizationResultCode.Success, first.Code);
            Assert.AreEqual(MonetizationResultCode.CooldownActive, second.Code);
        }

        [Test]
        public void SessionCapBlocksAfterLimit()
        {
            MonetizationSession session = CreateSession();

            MonetizationResult first = session.ShowInterstitial(
                InterstitialPlacement,
                Context(nowSeconds: 10, inCombat: false, terminalRuns: 1));
            MonetizationResult second = session.ShowInterstitial(
                InterstitialPlacement,
                Context(nowSeconds: 200, inCombat: false, terminalRuns: 1));
            MonetizationResult third = session.ShowInterstitial(
                InterstitialPlacement,
                Context(nowSeconds: 400, inCombat: false, terminalRuns: 1));

            Assert.AreEqual(MonetizationResultCode.Success, first.Code);
            Assert.AreEqual(MonetizationResultCode.Success, second.Code);
            Assert.AreEqual(MonetizationResultCode.SessionCapReached, third.Code);
        }

        [Test]
        public void NoAdsEntitlementBlocksForcedInterstitial()
        {
            MonetizationSession session = CreateSession(
                noAdsEntitlementProvider: new StaticNoAdsEntitlementProvider(true));

            MonetizationResult result = session.ShowInterstitial(
                InterstitialPlacement,
                Context(nowSeconds: 10, inCombat: false, terminalRuns: 1));

            Assert.AreEqual(MonetizationResultCode.BlockedByNoAdsEntitlement, result.Code);
        }

        [Test]
        public void DuplicateRewardedClaimDoesNotGrantTwice()
        {
            MonetizationSession session = CreateSession();
            var claim = new RewardClaimId("claim.duplicate");

            MonetizationResult first = session.ShowRewarded(RewardedPlacement, claim, Context(10, false, 1));
            MonetizationResult second = session.ShowRewarded(RewardedPlacement, claim, Context(20, false, 1));

            Assert.AreEqual(MonetizationResultCode.Success, first.Code);
            Assert.AreEqual(MonetizationResultCode.DuplicateClaim, second.Code);
        }

        [Test]
        public void ConsentGateBlocksAvailability()
        {
            MonetizationSession session = CreateSession(
                consentProvider: new StaticMonetizationConsentProvider(false));

            MonetizationAvailability availability = session.GetAvailability(
                RewardedPlacement,
                MonetizationPlacementKind.Rewarded,
                Context(10, false, 1));

            Assert.IsFalse(availability.IsAvailable);
            Assert.AreEqual(MonetizationResultCode.ConsentRequired, availability.Code);
        }

        private static MonetizationSession CreateSession(
            IMonetizationProvider provider = null,
            IMonetizationConsentProvider consentProvider = null,
            INoAdsEntitlementProvider noAdsEntitlementProvider = null)
        {
            return new MonetizationSession(
                new[]
                {
                    new MonetizationPlacementPolicy(RewardedPlacement, MonetizationPlacementKind.Rewarded, TimeSpan.Zero, 0),
                    new MonetizationPlacementPolicy(
                        InterstitialPlacement,
                        MonetizationPlacementKind.Interstitial,
                        TimeSpan.FromSeconds(60),
                        sessionCap: 2,
                        blockBeforeFirstCompletedOrFailedRun: true,
                        blockDuringCombat: true,
                        blockWhenNoAdsEntitled: true,
                        cooldownGroup: "interstitial.global"),
                    new MonetizationPlacementPolicy(
                        AlternateInterstitialPlacement,
                        MonetizationPlacementKind.Interstitial,
                        TimeSpan.FromSeconds(60),
                        sessionCap: 2,
                        blockBeforeFirstCompletedOrFailedRun: true,
                        blockDuringCombat: true,
                        blockWhenNoAdsEntitled: true,
                        cooldownGroup: "interstitial.global")
                },
                provider ?? new MockMonetizationProvider(),
                consentProvider,
                noAdsEntitlementProvider);
        }

        private static MonetizationFlowContext Context(int nowSeconds, bool inCombat, int terminalRuns)
        {
            return new MonetizationFlowContext(DateTimeOffset.UnixEpoch.AddSeconds(nowSeconds), inCombat, terminalRuns);
        }
    }
}
