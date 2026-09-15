using System;
using System.Linq;
using UnityEngine;

namespace Deucarian.Monetization.Unity
{
    public sealed class MonetizationDefinitionCatalog : ScriptableObject
    {
        public const string ResourcePath = "Deucarian/Definitions/MonetizationDefinitionCatalog";
        [SerializeField] private RewardedPlacementDefinitionAsset[] rewarded = Array.Empty<RewardedPlacementDefinitionAsset>();
        [SerializeField] private InterstitialPlacementDefinitionAsset[] interstitial = Array.Empty<InterstitialPlacementDefinitionAsset>();
        public static MonetizationDefinitionCatalog LoadProject() => Resources.Load<MonetizationDefinitionCatalog>(ResourcePath) ?? throw new InvalidOperationException("Create rewarded or interstitial placements in Definitions before loading the catalog.");
        public MonetizationSession CreateSession(IMonetizationProvider provider, IMonetizationConsentProvider consent = null, INoAdsEntitlementProvider entitlement = null, RewardClaimLedger claims = null)
        {
            if (rewarded.Any(x => x == null) || interstitial.Any(x => x == null)) throw new InvalidOperationException("The placement catalog contains missing definitions. Synchronize it in Definitions.");
            var policies = rewarded.Select(x => x.ToRuntimeDefinition()).Concat(interstitial.Select(x => x.ToRuntimeDefinition())).ToArray();
            if (policies.GroupBy(x => x.Id).Any(x => x.Count() > 1)) throw new InvalidOperationException("Placement IDs must be unique across rewarded and interstitial definitions.");
            return new MonetizationSession(policies, provider, consent, entitlement, claims);
        }
    }
}
