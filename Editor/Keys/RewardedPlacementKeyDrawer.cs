using System;
using Deucarian.Editor;
using UnityEditor;

namespace Deucarian.Monetization.Editor
{
    [CustomPropertyDrawer(typeof(RewardedPlacementKey), true)]
    public sealed class RewardedPlacementKeyDrawer : DeucarianKeyDrawer
    {
        public override Type KeyType => typeof(RewardedPlacementKey);
        public override Type DefinitionSetAttribute => typeof(RewardedPlacementKeySetAttribute);
        public override string SetupHint => "Select an existing RewardedPlacementKey; declare reusable keys once in a [RewardedPlacementKeySet] class.";
    }
}
