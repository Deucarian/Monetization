using System;
using System.Linq;
using Deucarian.Editor;
using Deucarian.Editor.Definitions;
using Deucarian.Monetization.Unity;
using UnityEditor;
using UnityEngine;

namespace Deucarian.Monetization.Editor.Definitions
{
    [Serializable]
    public sealed class RewardedPlacementDefinitionSpec : DeucarianDefinitionSpec
    {
        [DefinitionField("cooldownSeconds")] public double CooldownSeconds = 30d;
        [DefinitionField("sessionCap")] public int SessionCap = 5;
        [DefinitionField("blockBeforeFirstRun")] public bool BlockBeforeFirstRun = false;
        [DefinitionField("blockDuringCombat")] public bool BlockDuringCombat = false;
        [DefinitionField("blockWithNoAds")] public bool BlockWithNoAds = false;
    }
}
