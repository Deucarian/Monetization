# Simple usage

Copy the reference example into your project, add SimpleUsageExample and assign its scoped host references. Its serialized definition fields use the same typed keys as code.

Configure the host with the existing MonetizationSession and a callback capturing the current flow context. The configured placement must be rewarded. Use the same owner-issued RewardClaimId when retrying the same claim; the session retains consent, pacing and idempotency checks. ShowRewardedAsync is a Task adapter over the current synchronous provider contract, not a new asynchronous ad SDK integration.

Definitions are authored once in SampleDefinitions.cs where applicable; the caller never invents an ID. Replace the sample set with your project's central definitions. A selected key proves its identity and payload type; startup still needs to bind that definition in the correct scope. Missing configuration reports how to fix it. Dynamic targets and choices are issued by their owner instead of selected from a definition dropdown.
```csharp
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
```
