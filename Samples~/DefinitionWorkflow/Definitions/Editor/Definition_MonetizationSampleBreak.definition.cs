// <deucarian-definition schema="interstitial-placements" />
// Editable declaration. Use the package definition editor or edit the values below.
namespace Deucarian.ProjectDefinitions.Definition_interstitial_placements
{
    public static class Definition_MonetizationSampleBreak
    {
        public static global::Deucarian.Monetization.Editor.Definitions.InterstitialPlacementDefinitionSpec Value =>
        // definition-value
        new global::Deucarian.Monetization.Editor.Definitions.InterstitialPlacementDefinitionSpec
        {
            BlockBeforeFirstRun = false,
            BlockDuringCombat = true,
            BlockWithNoAds = true,
            CooldownSeconds = 0d,
            Id = "ac453ad72210401da319115a8b711876",
            Name = "MonetizationSampleBreak",
            SessionCap = 100,
        };
        // end-definition-value
    }
}
