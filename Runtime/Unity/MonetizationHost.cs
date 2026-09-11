using Deucarian.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Deucarian.Monetization.Unity
{
    /// <summary>Typed access to a configured session. Consent, pacing and claim deduplication remain in that session.</summary>
    [DisallowMultipleComponent]
    public sealed class MonetizationHost : MonoBehaviour, IDiagnosticProvider
    {
        private MonetizationSession session;
        private Func<MonetizationFlowContext> captureContext;
        private bool destroyed;

        public void Configure(MonetizationSession value, Func<MonetizationFlowContext> context)
        {
            if (destroyed) throw new ObjectDisposedException(nameof(MonetizationHost));
            if (session != null) throw new InvalidOperationException("MonetizationHost '" + name + "' is already configured.");
            if (value == null) throw new ArgumentNullException(nameof(value));
            captureContext = context ?? throw new ArgumentNullException(nameof(context));
            session = value;
        }

        public MonetizationResult ShowRewarded(RewardedPlacementKey placement, RewardClaimId claim)
        {
            if (placement == null) throw new ArgumentNullException(nameof(placement), "Select a rewarded placement or pass its named RewardedPlacementKey.");
            var id = RequirePlacement(placement.Id, MonetizationPlacementKind.Rewarded);
            if (claim.IsEmpty) throw new ArgumentException("Supply the reward opportunity's claim ID. Reuse that ID when retrying the same opportunity.", nameof(claim));
            return session.ShowRewarded(id, claim, captureContext());
        }

        /// <summary>Task adapter for the package's synchronous provider contract; no worker thread is created.</summary>
        public Task<MonetizationResult> ShowRewardedAsync(RewardedPlacementKey placement, RewardClaimId claim,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(ShowRewarded(placement, claim));
        }

        public MonetizationResult ShowInterstitial(InterstitialPlacementKey placement)
        {
            if (placement == null) throw new ArgumentNullException(nameof(placement), "Select an interstitial placement or pass its named InterstitialPlacementKey.");
            return session.ShowInterstitial(RequirePlacement(placement.Id, MonetizationPlacementKind.Interstitial), captureContext());
        }

        private MonetizationPlacementId RequirePlacement(string id, MonetizationPlacementKind kind)
        {
            if (destroyed) throw new ObjectDisposedException(nameof(MonetizationHost));
            if (session == null) throw new InvalidOperationException("MonetizationHost '" + name + "' is not configured. Supply its MonetizationSession and current flow context during startup.");
            var placement = new MonetizationPlacementId(id);
            if (!session.ContainsPlacement(placement, kind)) throw new InvalidOperationException("MonetizationHost '" + name + "' has no " + kind + " placement '" + id + "'. Register a matching placement policy in its MonetizationSession.");
            return placement;
        }
        private void OnDestroy() { diagnosticRegistration?.Dispose(); diagnosticRegistration = null;  destroyed = true; session = null; captureContext = null; }
        private DiagnosticProviderRegistration diagnosticRegistration;
        private void Awake() => diagnosticRegistration = DiagnosticProviderRegistry.Register(this);
        string IDiagnosticProvider.ProviderId => "monetization.host." + GetInstanceID();
        string IDiagnosticProvider.DisplayName => "MonetizationHost";
        void IDiagnosticProvider.Collect(DiagnosticReportBuilder builder)
        {
            bool configured = session != null;
            builder.AddSection(((IDiagnosticProvider)this).ProviderId, "MonetizationHost")
                .AddItem("configured", "Configured", configured ? "Ready" : "Call Configure during startup",
                    configured ? DiagnosticSeverity.Info : DiagnosticSeverity.Warning)
                .AddItem("enabled", "Enabled", isActiveAndEnabled.ToString());
        }
    }
}
