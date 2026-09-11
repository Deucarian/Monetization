namespace Deucarian.Monetization.Samples.SimpleUsage
{
    [RewardedPlacementKeySet]
    public static class AdPlacements
    {
        public static RewardedPlacementKey Revive => new Definition();
        private sealed class Definition : RewardedPlacementKey
        {
            public Definition() : base("sample.revive") { }
        }
    }
}
