namespace Deucarian.Monetization.Samples
{
    /// <summary>Shows how a game can make a rewarded claim idempotent.</summary>
    public static class RewardedClaimPrimer
    {
        public static bool RecordExactlyOnce()
        {
            var ledger = new RewardClaimLedger();
            var claim = new RewardClaimId("sample.rewarded.continue");

            return ledger.TryRecord(claim) && !ledger.TryRecord(claim);
        }
    }
}
