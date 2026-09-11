using System;
using UnityEngine;
using UnityEngine.Events;

namespace Deucarian.Monetization.Unity
{
    [AddComponentMenu("Deucarian/Monetization/Interstitial Ad Trigger")]
    public sealed class InterstitialAdTrigger : MonoBehaviour
    {
        [SerializeField] private MonetizationHost host;
        [SerializeField] private InterstitialPlacementKey placement;
        [SerializeField] private UnityEvent shown = new UnityEvent();
        [SerializeField] private UnityEvent<string> unavailable = new UnityEvent<string>();
        public MonetizationResult ShowInterstitial()
        {
            if (host == null) throw new InvalidOperationException("Assign a configured MonetizationHost to InterstitialAdTrigger '" + name + "'.");
            return host.ShowInterstitial(placement);
        }
        public void Show()
        {
            var result = ShowInterstitial();
            if (result.Succeeded) shown.Invoke(); else unavailable.Invoke(result.Message);
        }
    }
}
