using System;
using UnityEngine;
namespace Deucarian.Monetization.Unity.Samples.DefinitionWorkflow
{
    [DefaultExecutionOrder(-2000)]
    public sealed class SampleMonetizationSetup : MonoBehaviour
    {
        [SerializeField] private MonetizationHost host;
        private void Awake() => host.Configure(MonetizationDefinitionCatalog.LoadProject().CreateSession(new MockMonetizationProvider()),
            () => new MonetizationFlowContext(DateTimeOffset.UtcNow, false, 1));
    }
}
