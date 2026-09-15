using System;

using UnityEngine;

namespace Deucarian.Monetization.Unity
{
    /// <summary>Reusable defaults for code calls and serialized typed keys; runtime state remains in the core service.</summary>
    public sealed class RewardedPlacementDefinitionAsset : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private double cooldownSeconds = 30d;
        [SerializeField] private int sessionCap = 5;
        [SerializeField] private bool blockBeforeFirstRun = false;
        [SerializeField] private bool blockDuringCombat = false;
        [SerializeField] private bool blockWithNoAds = false;
        public string Id => id;
        public string DisplayName => displayName;
        public RewardedPlacementKey Key => new AssetKey(id);
        public MonetizationPlacementPolicy ToRuntimeDefinition()
        {
            if (double.IsNaN(cooldownSeconds) || double.IsInfinity(cooldownSeconds) || cooldownSeconds < 0 || sessionCap < 0) throw new InvalidOperationException("Set a finite non-negative cooldown and session cap for placement '" + DisplayName + "'.");
            return new MonetizationPlacementPolicy(new MonetizationPlacementId(Id), MonetizationPlacementKind.Rewarded, TimeSpan.FromSeconds(cooldownSeconds), sessionCap, blockBeforeFirstRun, blockDuringCombat, blockWithNoAds);
        }
        private sealed class AssetKey : RewardedPlacementKey { public AssetKey(string value) : base(value) { } }
    }
}
