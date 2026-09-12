using System;
using UnityEngine;

namespace Deucarian.Monetization.Unity.Samples.DefinitionWorkflow
{
    /// <summary>Small caller example. The configured scene hosts own services and resource lifetimes.</summary>
    public sealed class MonetizationWorkflow : MonoBehaviour
    {
        [SerializeField] private RewardedAdTrigger rewarded;
        [SerializeField] private InterstitialAdTrigger interstitial;
        private string status = "Ready. Choose an action below.";
        public string Status => status;
        public void ShowRewarded() { status = "Rewarded result: " + rewarded.ShowRewarded().Code; }
        public void NewOpportunity() { rewarded.BeginNewOpportunity(); ShowRewarded(); }
        public void ShowInterstitial() { status = "Interstitial result: " + interstitial.ShowInterstitial().Code; }
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(24, 24, Math.Min(540, Screen.width - 48), Screen.height - 48), GUI.skin.box);
            GUILayout.Label("Monetization — definition workflow");
            GUILayout.Label("Placement definitions own pacing and gating defaults. This scene uses the package mock provider; no real advertisements or purchases take place.");
            GUILayout.Space(12);
            if (GUILayout.Button("Show rewarded mock", GUILayout.Height(32))) { try { ShowRewarded(); } catch (Exception error) { status = error.Message; } }
            if (GUILayout.Button("Retry same opportunity", GUILayout.Height(32))) { try { ShowRewarded(); } catch (Exception error) { status = error.Message; } }
            if (GUILayout.Button("New opportunity", GUILayout.Height(32))) { try { NewOpportunity(); } catch (Exception error) { status = error.Message; } }
            if (GUILayout.Button("Show interstitial mock", GUILayout.Height(32))) { try { ShowInterstitial(); } catch (Exception error) { status = error.Message; } }
            GUILayout.Space(12);
            GUILayout.Label(status);
            GUILayout.EndArea();
        }
    }
}
