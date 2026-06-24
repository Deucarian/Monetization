# Monetization API

Use `MonetizationSession` to evaluate placement pacing and call an injected provider.

Important types:

- `MonetizationPlacementId`
- `RewardClaimId`
- `MonetizationPlacementPolicy`
- `MonetizationFlowContext`
- `MonetizationSession`
- `NoOpMonetizationProvider`
- `MockMonetizationProvider`

The package is intentionally SDK-free. Real providers should adapt ad SDKs behind `IMonetizationProvider` in a separate integration package.
