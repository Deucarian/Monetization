using System;
using System.Linq;
using Deucarian.Editor.Definitions;
using Deucarian.Monetization.Unity;
using UnityEditor;

namespace Deucarian.Monetization.Editor.Definitions
{
    internal static class ProjectDefinitionCatalogAuthoring
    {
        internal static void Refresh(bool validateOnly = false)
        {
            var rewarded = AssetDatabase.FindAssets("t:RewardedPlacementDefinitionAsset", new[] { "Assets" }).Select(x => AssetDatabase.LoadAssetAtPath<RewardedPlacementDefinitionAsset>(AssetDatabase.GUIDToAssetPath(x))).Where(x => x != null).OrderBy(x => x.Id, StringComparer.Ordinal).ToArray();
            var interstitial = AssetDatabase.FindAssets("t:InterstitialPlacementDefinitionAsset", new[] { "Assets" }).Select(x => AssetDatabase.LoadAssetAtPath<InterstitialPlacementDefinitionAsset>(AssetDatabase.GUIDToAssetPath(x))).Where(x => x != null).OrderBy(x => x.Id, StringComparer.Ordinal).ToArray();
            if (rewarded.Select(x => x.Id).Concat(interstitial.Select(x => x.Id)).GroupBy(x => x).Any(x => x.Count() > 1)) throw new InvalidOperationException("Placement IDs must be unique.");
            const string path = "Assets/DeucarianDefinitions/Resources/Deucarian/Definitions/MonetizationDefinitionCatalog.asset";
            DeucarianDefinitionCatalog.Update<MonetizationDefinitionCatalog>(path, "rewarded", rewarded, validateOnly);
            DeucarianDefinitionCatalog.Update<MonetizationDefinitionCatalog>(path, "interstitial", interstitial, validateOnly);
        }
    }
}
