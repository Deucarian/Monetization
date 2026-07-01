# Deucarian Monetization

`com.deucarian.monetization` defines SDK-free monetization primitives for Deucarian mobile game templates and products.

It owns placement IDs, rewarded and interstitial ad abstractions, no-op and mock providers, pacing policies, consent/availability gates, no-ads entitlement checks, and rewarded claim identity.

It does not include real ad SDKs, real billing, analytics, store configuration, privacy policy generation, UI, game-specific rewards, or save files.

## Install

Stable:

```json
"com.deucarian.monetization": "https://github.com/Deucarian/Monetization.git#main"
```

Development:

```json
"com.deucarian.monetization": "https://github.com/Deucarian/Monetization.git#develop"
```

Use `#main` for stable package consumption and `#develop` when testing active package work.

## When To Use This

Use this package when you need SDK-free monetization placement abstractions, mock/no-op ad providers, pacing policies, entitlements, consent, and rewarded claim identity for Deucarian games.

Do not use this package to take ownership of capabilities outside its `AGENTS.md` boundary. Reusable behavior should stay with the package that owns that capability in the Package Registry governance docs.

## Quick Start

1. Install the package through Deucarian Package Installer or Unity Package Manager using the URL above.
2. Let Unity finish resolving packages and compiling assemblies.
3. Start from the package README sections above and the public runtime/editor APIs in this repository.

## Integrations

This package has no direct Deucarian package dependencies.

Install optional companion packages only when their owned capability is needed by production code, samples, or tests.

## Validation

Run the shared package validator from this repository root:

```powershell
python C:/Repositories/Package-Registry/Tools/deucarian_package_validator.py --registry-root C:/Repositories/Package-Registry --repository-root . --config deucarian-package.json
```

Documentation-only updates should still pass:

```powershell
git diff --check
```

## Troubleshooting

- Package does not resolve: confirm the stable or development Git URL matches the Package Registry entry and that required Deucarian dependencies are installed.
- Unity compile errors after install: let Package Manager finish resolving dependencies, then check asmdef references against `package.json` dependencies.
- Behavior appears to belong in another package: consult `AGENTS.md` and the Package Registry governance docs before moving or duplicating code.
