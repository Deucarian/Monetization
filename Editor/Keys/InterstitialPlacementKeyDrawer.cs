using System;
using Deucarian.Editor;
using UnityEditor;

namespace Deucarian.Monetization.Editor
{
    [CustomPropertyDrawer(typeof(InterstitialPlacementKey), true)]
    public sealed class InterstitialPlacementKeyDrawer : DeucarianKeyDrawer
    {
        public override Type KeyType => typeof(InterstitialPlacementKey);
        public override Type DefinitionSetAttribute => typeof(InterstitialPlacementKeySetAttribute);
        public override string SetupHint => "Select an existing InterstitialPlacementKey; declare reusable keys once in a [InterstitialPlacementKeySet] class.";
    }
}
