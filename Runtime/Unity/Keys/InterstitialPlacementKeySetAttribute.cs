using System;

namespace Deucarian.Monetization
{
    /// <summary>Marks an authoritative set of named InterstitialPlacementKey fields or properties for the Inspector.</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class InterstitialPlacementKeySetAttribute : Attribute { }
}
