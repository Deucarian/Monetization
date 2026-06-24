using System;
using System.Collections.Generic;

namespace Deucarian.Monetization
{
    public readonly struct MonetizationPlacementId : IEquatable<MonetizationPlacementId>
    {
        public MonetizationPlacementId(string value)
        {
            Value = value ?? string.Empty;
        }

        public string Value { get; }
        public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
        public bool Equals(MonetizationPlacementId other) => string.Equals(Value, other.Value, StringComparison.Ordinal);
        public override bool Equals(object obj) => obj is MonetizationPlacementId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
    }

    public readonly struct RewardClaimId : IEquatable<RewardClaimId>
    {
        public RewardClaimId(string value)
        {
            Value = value ?? string.Empty;
        }

        public string Value { get; }
        public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
        public bool Equals(RewardClaimId other) => string.Equals(Value, other.Value, StringComparison.Ordinal);
        public override bool Equals(object obj) => obj is RewardClaimId other && Equals(other);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value ?? string.Empty;
    }

    public enum MonetizationPlacementKind
    {
        Rewarded = 0,
        Interstitial = 1,
        PurchasePlaceholder = 2
    }

    public enum MonetizationResultCode
    {
        Success = 0,
        Unavailable = 1,
        Cancelled = 2,
        CooldownActive = 3,
        SessionCapReached = 4,
        BlockedDuringCombat = 5,
        BlockedBeforeFirstRun = 6,
        BlockedByNoAdsEntitlement = 7,
        DuplicateClaim = 8,
        ConsentRequired = 9,
        InvalidPlacement = 10,
        ProviderError = 11,
        NoOp = 12
    }

    public sealed class MonetizationAvailability
    {
        private MonetizationAvailability(bool available, MonetizationResultCode code, string message)
        {
            IsAvailable = available;
            Code = code;
            Message = message ?? string.Empty;
        }

        public bool IsAvailable { get; }
        public MonetizationResultCode Code { get; }
        public string Message { get; }

        public static MonetizationAvailability Available()
        {
            return new MonetizationAvailability(true, MonetizationResultCode.Success, string.Empty);
        }

        public static MonetizationAvailability Blocked(MonetizationResultCode code, string message)
        {
            return new MonetizationAvailability(false, code, message);
        }
    }

    public sealed class MonetizationResult
    {
        private MonetizationResult(MonetizationPlacementId placementId, RewardClaimId claimId, MonetizationResultCode code, string message)
        {
            PlacementId = placementId;
            ClaimId = claimId;
            Code = code;
            Message = message ?? string.Empty;
        }

        public MonetizationPlacementId PlacementId { get; }
        public RewardClaimId ClaimId { get; }
        public MonetizationResultCode Code { get; }
        public string Message { get; }
        public bool Succeeded => Code == MonetizationResultCode.Success;

        public static MonetizationResult From(MonetizationPlacementId placementId, MonetizationResultCode code, string message = "")
        {
            return new MonetizationResult(placementId, default, code, message);
        }

        public static MonetizationResult FromRewarded(MonetizationPlacementId placementId, RewardClaimId claimId, MonetizationResultCode code, string message = "")
        {
            return new MonetizationResult(placementId, claimId, code, message);
        }
    }

    public interface IMonetizationProvider
    {
        MonetizationAvailability GetAvailability(MonetizationPlacementId placementId, MonetizationPlacementKind kind);
        MonetizationResult ShowRewarded(MonetizationPlacementId placementId, RewardClaimId claimId);
        MonetizationResult ShowInterstitial(MonetizationPlacementId placementId);
    }

    public interface IMonetizationConsentProvider
    {
        bool CanRequestAds { get; }
    }

    public interface INoAdsEntitlementProvider
    {
        bool HasNoAdsEntitlement { get; }
    }

    public sealed class StaticMonetizationConsentProvider : IMonetizationConsentProvider
    {
        public StaticMonetizationConsentProvider(bool canRequestAds)
        {
            CanRequestAds = canRequestAds;
        }

        public bool CanRequestAds { get; set; }
    }

    public sealed class StaticNoAdsEntitlementProvider : INoAdsEntitlementProvider
    {
        public StaticNoAdsEntitlementProvider(bool hasNoAdsEntitlement)
        {
            HasNoAdsEntitlement = hasNoAdsEntitlement;
        }

        public bool HasNoAdsEntitlement { get; set; }
    }

    public readonly struct MonetizationFlowContext
    {
        public MonetizationFlowContext(DateTimeOffset nowUtc, bool isInCombat, int completedOrFailedRunCount)
        {
            NowUtc = nowUtc;
            IsInCombat = isInCombat;
            CompletedOrFailedRunCount = completedOrFailedRunCount;
        }

        public DateTimeOffset NowUtc { get; }
        public bool IsInCombat { get; }
        public int CompletedOrFailedRunCount { get; }
    }

    public sealed class MonetizationPlacementPolicy
    {
        public MonetizationPlacementPolicy(
            MonetizationPlacementId id,
            MonetizationPlacementKind kind,
            TimeSpan cooldown,
            int sessionCap,
            bool blockBeforeFirstCompletedOrFailedRun = false,
            bool blockDuringCombat = false,
            bool blockWhenNoAdsEntitled = false,
            string cooldownGroup = "")
        {
            Id = id;
            Kind = kind;
            Cooldown = cooldown < TimeSpan.Zero ? TimeSpan.Zero : cooldown;
            SessionCap = sessionCap < 0 ? 0 : sessionCap;
            BlockBeforeFirstCompletedOrFailedRun = blockBeforeFirstCompletedOrFailedRun;
            BlockDuringCombat = blockDuringCombat;
            BlockWhenNoAdsEntitled = blockWhenNoAdsEntitled;
            CooldownGroup = cooldownGroup ?? string.Empty;
        }

        public MonetizationPlacementId Id { get; }
        public MonetizationPlacementKind Kind { get; }
        public TimeSpan Cooldown { get; }
        public int SessionCap { get; }
        public bool BlockBeforeFirstCompletedOrFailedRun { get; }
        public bool BlockDuringCombat { get; }
        public bool BlockWhenNoAdsEntitled { get; }
        public string CooldownGroup { get; }
    }

    public sealed class RewardClaimLedger
    {
        private readonly HashSet<RewardClaimId> _claimed = new HashSet<RewardClaimId>();

        public bool HasClaimed(RewardClaimId claimId)
        {
            return !claimId.IsEmpty && _claimed.Contains(claimId);
        }

        public bool TryRecord(RewardClaimId claimId)
        {
            if (claimId.IsEmpty) return true;
            return _claimed.Add(claimId);
        }
    }

    public sealed class MonetizationSession
    {
        private readonly Dictionary<MonetizationPlacementId, MonetizationPlacementPolicy> _policies =
            new Dictionary<MonetizationPlacementId, MonetizationPlacementPolicy>();
        private readonly Dictionary<MonetizationPlacementId, int> _impressions =
            new Dictionary<MonetizationPlacementId, int>();
        private readonly Dictionary<string, DateTimeOffset> _lastShownUtc =
            new Dictionary<string, DateTimeOffset>(StringComparer.Ordinal);
        private readonly RewardClaimLedger _claimLedger;
        private readonly IMonetizationProvider _provider;
        private readonly IMonetizationConsentProvider _consentProvider;
        private readonly INoAdsEntitlementProvider _noAdsEntitlementProvider;

        public MonetizationSession(
            IEnumerable<MonetizationPlacementPolicy> policies,
            IMonetizationProvider provider,
            IMonetizationConsentProvider consentProvider = null,
            INoAdsEntitlementProvider noAdsEntitlementProvider = null,
            RewardClaimLedger claimLedger = null)
        {
            _provider = provider ?? new NoOpMonetizationProvider();
            _consentProvider = consentProvider ?? new StaticMonetizationConsentProvider(true);
            _noAdsEntitlementProvider = noAdsEntitlementProvider ?? new StaticNoAdsEntitlementProvider(false);
            _claimLedger = claimLedger ?? new RewardClaimLedger();

            if (policies == null) return;
            foreach (MonetizationPlacementPolicy policy in policies)
            {
                if (policy == null || policy.Id.IsEmpty) continue;
                _policies[policy.Id] = policy;
            }
        }

        public MonetizationAvailability GetAvailability(
            MonetizationPlacementId placementId,
            MonetizationPlacementKind kind,
            MonetizationFlowContext context)
        {
            if (!TryGetPolicy(placementId, kind, out MonetizationPlacementPolicy policy, out MonetizationAvailability blocked))
                return blocked;

            blocked = EvaluatePolicy(policy, context);
            if (!blocked.IsAvailable) return blocked;

            return _provider.GetAvailability(placementId, kind);
        }

        public MonetizationResult ShowRewarded(
            MonetizationPlacementId placementId,
            RewardClaimId claimId,
            MonetizationFlowContext context)
        {
            if (!TryGetPolicy(placementId, MonetizationPlacementKind.Rewarded, out MonetizationPlacementPolicy policy, out MonetizationAvailability blocked))
                return MonetizationResult.FromRewarded(placementId, claimId, blocked.Code, blocked.Message);

            blocked = EvaluatePolicy(policy, context);
            if (!blocked.IsAvailable) return MonetizationResult.FromRewarded(placementId, claimId, blocked.Code, blocked.Message);

            if (_claimLedger.HasClaimed(claimId))
                return MonetizationResult.FromRewarded(placementId, claimId, MonetizationResultCode.DuplicateClaim, "Reward claim has already been granted.");

            MonetizationAvailability providerAvailability = _provider.GetAvailability(placementId, MonetizationPlacementKind.Rewarded);
            if (!providerAvailability.IsAvailable)
                return MonetizationResult.FromRewarded(placementId, claimId, providerAvailability.Code, providerAvailability.Message);

            MonetizationResult result = _provider.ShowRewarded(placementId, claimId);
            if (!result.Succeeded) return result;

            if (!_claimLedger.TryRecord(claimId))
                return MonetizationResult.FromRewarded(placementId, claimId, MonetizationResultCode.DuplicateClaim, "Reward claim has already been granted.");

            RecordImpression(policy, context.NowUtc);
            return result;
        }

        public MonetizationResult ShowInterstitial(MonetizationPlacementId placementId, MonetizationFlowContext context)
        {
            if (!TryGetPolicy(placementId, MonetizationPlacementKind.Interstitial, out MonetizationPlacementPolicy policy, out MonetizationAvailability blocked))
                return MonetizationResult.From(placementId, blocked.Code, blocked.Message);

            blocked = EvaluatePolicy(policy, context);
            if (!blocked.IsAvailable) return MonetizationResult.From(placementId, blocked.Code, blocked.Message);

            MonetizationAvailability providerAvailability = _provider.GetAvailability(placementId, MonetizationPlacementKind.Interstitial);
            if (!providerAvailability.IsAvailable)
                return MonetizationResult.From(placementId, providerAvailability.Code, providerAvailability.Message);

            MonetizationResult result = _provider.ShowInterstitial(placementId);
            if (result.Succeeded) RecordImpression(policy, context.NowUtc);
            return result;
        }

        private bool TryGetPolicy(
            MonetizationPlacementId placementId,
            MonetizationPlacementKind kind,
            out MonetizationPlacementPolicy policy,
            out MonetizationAvailability blocked)
        {
            policy = null;
            if (placementId.IsEmpty || !_policies.TryGetValue(placementId, out policy) || policy.Kind != kind)
            {
                blocked = MonetizationAvailability.Blocked(MonetizationResultCode.InvalidPlacement, "Placement is not registered for this kind.");
                return false;
            }

            blocked = MonetizationAvailability.Available();
            return true;
        }

        private MonetizationAvailability EvaluatePolicy(MonetizationPlacementPolicy policy, MonetizationFlowContext context)
        {
            if (!_consentProvider.CanRequestAds)
                return MonetizationAvailability.Blocked(MonetizationResultCode.ConsentRequired, "Consent or availability gate blocked ads.");

            if (policy.BlockDuringCombat && context.IsInCombat)
                return MonetizationAvailability.Blocked(MonetizationResultCode.BlockedDuringCombat, "Placement is blocked during combat.");

            if (policy.BlockBeforeFirstCompletedOrFailedRun && context.CompletedOrFailedRunCount <= 0)
                return MonetizationAvailability.Blocked(MonetizationResultCode.BlockedBeforeFirstRun, "Placement is blocked before the first terminal run.");

            if (policy.BlockWhenNoAdsEntitled && _noAdsEntitlementProvider.HasNoAdsEntitlement)
                return MonetizationAvailability.Blocked(MonetizationResultCode.BlockedByNoAdsEntitlement, "No-ads entitlement blocks forced interstitials.");

            if (policy.SessionCap > 0 && _impressions.TryGetValue(policy.Id, out int count) && count >= policy.SessionCap)
                return MonetizationAvailability.Blocked(MonetizationResultCode.SessionCapReached, "Placement session cap reached.");

            string cooldownKey = GetCooldownKey(policy);
            if (policy.Cooldown > TimeSpan.Zero &&
                _lastShownUtc.TryGetValue(cooldownKey, out DateTimeOffset lastShown) &&
                context.NowUtc - lastShown < policy.Cooldown)
                return MonetizationAvailability.Blocked(MonetizationResultCode.CooldownActive, "Placement cooldown is active.");

            return MonetizationAvailability.Available();
        }

        private void RecordImpression(MonetizationPlacementPolicy policy, DateTimeOffset nowUtc)
        {
            _lastShownUtc[GetCooldownKey(policy)] = nowUtc;
            _impressions.TryGetValue(policy.Id, out int count);
            _impressions[policy.Id] = count + 1;
        }

        private static string GetCooldownKey(MonetizationPlacementPolicy policy)
        {
            return string.IsNullOrEmpty(policy.CooldownGroup) ? policy.Id.Value : policy.CooldownGroup;
        }
    }

    public sealed class NoOpMonetizationProvider : IMonetizationProvider
    {
        public MonetizationAvailability GetAvailability(MonetizationPlacementId placementId, MonetizationPlacementKind kind)
        {
            return MonetizationAvailability.Blocked(MonetizationResultCode.NoOp, "No-op monetization provider is active.");
        }

        public MonetizationResult ShowRewarded(MonetizationPlacementId placementId, RewardClaimId claimId)
        {
            return MonetizationResult.FromRewarded(placementId, claimId, MonetizationResultCode.NoOp, "No-op monetization provider is active.");
        }

        public MonetizationResult ShowInterstitial(MonetizationPlacementId placementId)
        {
            return MonetizationResult.From(placementId, MonetizationResultCode.NoOp, "No-op monetization provider is active.");
        }
    }

    public sealed class MockMonetizationProvider : IMonetizationProvider
    {
        private readonly Dictionary<MonetizationPlacementId, MonetizationResultCode> _availability =
            new Dictionary<MonetizationPlacementId, MonetizationResultCode>();
        private readonly Dictionary<MonetizationPlacementId, Queue<MonetizationResultCode>> _showResults =
            new Dictionary<MonetizationPlacementId, Queue<MonetizationResultCode>>();

        public MonetizationResultCode DefaultAvailability { get; set; } = MonetizationResultCode.Success;
        public MonetizationResultCode DefaultShowResult { get; set; } = MonetizationResultCode.Success;

        public void SetAvailability(MonetizationPlacementId placementId, MonetizationResultCode code)
        {
            _availability[placementId] = code;
        }

        public void EnqueueShowResult(MonetizationPlacementId placementId, MonetizationResultCode code)
        {
            if (!_showResults.TryGetValue(placementId, out Queue<MonetizationResultCode> queue))
            {
                queue = new Queue<MonetizationResultCode>();
                _showResults[placementId] = queue;
            }

            queue.Enqueue(code);
        }

        public MonetizationAvailability GetAvailability(MonetizationPlacementId placementId, MonetizationPlacementKind kind)
        {
            MonetizationResultCode code = _availability.TryGetValue(placementId, out MonetizationResultCode configured)
                ? configured
                : DefaultAvailability;
            return code == MonetizationResultCode.Success
                ? MonetizationAvailability.Available()
                : MonetizationAvailability.Blocked(code, "Mock availability returned " + code + ".");
        }

        public MonetizationResult ShowRewarded(MonetizationPlacementId placementId, RewardClaimId claimId)
        {
            MonetizationResultCode code = DequeueResult(placementId);
            return MonetizationResult.FromRewarded(placementId, claimId, code, "Mock rewarded returned " + code + ".");
        }

        public MonetizationResult ShowInterstitial(MonetizationPlacementId placementId)
        {
            MonetizationResultCode code = DequeueResult(placementId);
            return MonetizationResult.From(placementId, code, "Mock interstitial returned " + code + ".");
        }

        private MonetizationResultCode DequeueResult(MonetizationPlacementId placementId)
        {
            if (_showResults.TryGetValue(placementId, out Queue<MonetizationResultCode> queue) && queue.Count > 0)
                return queue.Dequeue();
            return DefaultShowResult;
        }
    }
}
