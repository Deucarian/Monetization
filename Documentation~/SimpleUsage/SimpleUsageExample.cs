using UnityEngine;
using Deucarian.Monetization.Unity;
namespace Deucarian.Monetization.Samples.SimpleUsage
{
    public sealed class SimpleUsageExample : MonoBehaviour
    {
        [SerializeField] private MonetizationHost ads;
        [SerializeField] private RewardedPlacementKey placement = AdPlacements.Revive;
        public MonetizationResult OfferRevive(RewardClaimId claim) => ads.ShowRewarded(placement, claim);
    }
}
