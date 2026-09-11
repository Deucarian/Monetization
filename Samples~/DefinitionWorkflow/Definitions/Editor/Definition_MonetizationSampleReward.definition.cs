// <deucarian-definition schema="rewarded-placements" />
// Editable declaration. Use the package definition editor or edit the values below.
namespace Deucarian.ProjectDefinitions.Definition_rewarded_placements
{
    public static class Definition_MonetizationSampleReward
    {
        public static global::Deucarian.Monetization.Editor.Definitions.RewardedPlacementDefinitionSpec Value =>
        // definition-value
        new global::Deucarian.Monetization.Editor.Definitions.RewardedPlacementDefinitionSpec
        {
            BlockBeforeFirstRun = false,
            BlockDuringCombat = false,
            BlockWithNoAds = false,
            CooldownSeconds = 0d,
            Id = "50ad58ba4b214dc1b0f1f098b2477c87",
            Name = "MonetizationSampleReward",
            SessionCap = 100,
        };
        // end-definition-value
    }
}
