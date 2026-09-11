using System;
using System.Linq;
using Deucarian.Editor;
using Deucarian.Editor.Definitions;
using Deucarian.Monetization.Unity;
using UnityEditor;
using UnityEngine;

namespace Deucarian.Monetization.Editor.Definitions
{
    public sealed class RewardedPlacementKeySource : DeucarianAssetKeySource<RewardedPlacementDefinitionAsset>
    {
        public override Type KeyType => typeof(RewardedPlacementKey);
        public override Type DefinitionSetAttribute => typeof(RewardedPlacementKeySetAttribute);
        public override string GeneratedClassName => "ProjectRewardedPlacements";
        protected override DeucarianKeyChoice ReadDefinition(RewardedPlacementDefinitionAsset asset) => new DeucarianKeyChoice(asset.Id, asset.DisplayName);
    }
}
