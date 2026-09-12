using System;
using System.Linq;
using Deucarian.Editor;
using Deucarian.Editor.Definitions;
using Deucarian.Monetization.Unity;
using UnityEditor;
using UnityEngine;

namespace Deucarian.Monetization.Editor.Definitions
{
    public sealed class InterstitialPlacementKeySource : DeucarianAssetKeySource<InterstitialPlacementDefinitionAsset>
    {
        public override Type KeyType => typeof(InterstitialPlacementKey);
        public override Type DefinitionSetAttribute => typeof(InterstitialPlacementKeySetAttribute);
        public override string GeneratedClassName => "ProjectInterstitialPlacements";
        protected override DeucarianKeyChoice ReadDefinition(InterstitialPlacementDefinitionAsset asset) => new DeucarianKeyChoice(asset.Id, asset.DisplayName);
    }
}
