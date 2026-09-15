using System;
using System.Linq;
using Deucarian.Editor;
using Deucarian.Editor.Definitions;
using Deucarian.Monetization.Unity;
using UnityEditor;
using UnityEngine;

namespace Deucarian.Monetization.Editor.Definitions
{
    public sealed class RewardedPlacementDefinitionSchema : DeucarianSerializedDefinitionSchema<RewardedPlacementDefinitionAsset, RewardedPlacementDefinitionSpec>
    {
        public override string Id => "rewarded-placements";
        public override string DisplayName => "Rewarded placements";
        public override void ValidateAssetReady(ScriptableObject asset)
        {
            base.ValidateAssetReady(asset);
            ((RewardedPlacementDefinitionAsset)asset).ToRuntimeDefinition();
        }
        public override void RefreshCatalog(bool validateOnly = false) { ProjectDefinitionCatalogAuthoring.Refresh(validateOnly); }
        [MenuItem("Assets/Create/Deucarian/Monetization/RewardedPlacement Definition")]
        private static void CreateDefinition() { Selection.activeObject = DeucarianDefinitionSync.Create(new RewardedPlacementDefinitionSchema(), "NewRewardedPlacement"); }
    }
}
