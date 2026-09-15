using System;
using System.Linq;
using Deucarian.Editor;
using Deucarian.Editor.Definitions;
using Deucarian.Monetization.Unity;
using UnityEditor;
using UnityEngine;

namespace Deucarian.Monetization.Editor.Definitions
{
    public sealed class InterstitialPlacementDefinitionSchema : DeucarianSerializedDefinitionSchema<InterstitialPlacementDefinitionAsset, InterstitialPlacementDefinitionSpec>
    {
        public override string Id => "interstitial-placements";
        public override string DisplayName => "Interstitial placements";
        public override void ValidateAssetReady(ScriptableObject asset)
        {
            base.ValidateAssetReady(asset);
            ((InterstitialPlacementDefinitionAsset)asset).ToRuntimeDefinition();
        }
        public override void RefreshCatalog(bool validateOnly = false) { ProjectDefinitionCatalogAuthoring.Refresh(validateOnly); }
        [MenuItem("Assets/Create/Deucarian/Monetization/InterstitialPlacement Definition")]
        private static void CreateDefinition() { Selection.activeObject = DeucarianDefinitionSync.Create(new InterstitialPlacementDefinitionSchema(), "NewInterstitialPlacement"); }
    }
}
