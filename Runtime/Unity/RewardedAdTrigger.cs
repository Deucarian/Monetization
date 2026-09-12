using System;
using UnityEngine;
using UnityEngine.Events;

namespace Deucarian.Monetization.Unity
{
    /// <summary>One reward opportunity. Retrying retains its claim; a new opportunity is explicit.</summary>
    [AddComponentMenu("Deucarian/Monetization/Rewarded Ad Trigger")]
    public sealed class RewardedAdTrigger : MonoBehaviour
    {
        [SerializeField] private MonetizationHost host;
        [SerializeField] private RewardedPlacementKey placement;
        [SerializeField] private UnityEvent rewardEarned = new UnityEvent();
        [SerializeField] private UnityEvent<string> unavailable = new UnityEvent<string>();
        private RewardClaimId claim;
        public MonetizationResult ShowRewarded()
        {
            if (host == null) throw new InvalidOperationException("Assign a configured MonetizationHost to RewardedAdTrigger '" + name + "'.");
            if (claim.IsEmpty) BeginNewOpportunity();
            return host.ShowRewarded(placement, claim);
        }
        public void Show()
        {
            var result = ShowRewarded();
            if (result.Succeeded) rewardEarned.Invoke(); else unavailable.Invoke(result.Message);
        }
        public void BeginNewOpportunity() => claim = new RewardClaimId(Guid.NewGuid().ToString("N"));
    }
}
