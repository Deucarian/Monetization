# Deucarian Monetization Agent Notes

Package ID: `com.deucarian.monetization`
Repository: `Deucarian/Monetization`

Follow the canonical Deucarian governance docs in [Package Registry](https://github.com/Deucarian/Package-Registry/blob/main/ARCHITECTURE.md), especially capability ownership and dependency rules.

## Ownership

This package owns:

- SDK-free monetization primitives, placement IDs, rewarded/interstitial abstractions, no-op and mock providers, pacing policies, consent/availability gates, no-ads entitlement checks, and rewarded claim identity.

Registered capabilities:
- None.

This package must not own:

- Real ad SDK adapters, billing/store implementations, analytics, privacy policy generation, UI flows, save files, or game-specific reward logic.

## Dependencies

Allowed dependency shape:

- Intentionally dependency-free so templates and products can compose provider adapters explicitly.

Required dependencies and why:

- None.

Optional/version-defined dependencies:

- None.

Architecture exceptions:

- Self-contained exception is recorded in `deucarian-package.json`; keep this package SDK-free and dependency-free unless governance changes.

## Policies

- SDK adapters: Keep third-party SDK integration in product-specific adapter packages or applications, not here.
- UI and persistence: Do not own monetization UI flows or save-file behavior.
- Logging: Do not introduce direct Unity Debug calls.
- Testing: Keep pacing, entitlement, consent, availability, mock/no-op provider, and rewarded-claim behavior covered by EditMode tests.

## Validation

Run the shared validator before committing:

```powershell
python C:/Repositories/Package-Registry/Tools/deucarian_package_validator.py --registry-root C:/Repositories/Package-Registry --repository-root . --config deucarian-package.json
```

Also run existing repository tests when changing code or asmdefs. Documentation-only updates should still run `git diff --check`.

## Codex Guidance

- Inspect current files before changing anything.
- Work on `develop`; do not edit or merge `main` unless the task is promotion-only.
- Do not edit `Library/PackageCache`.
- Do not guess package versions or dependency versions.
- Do not add package dependencies casually; update asmdefs, `package.json`, `deucarian-package.json`, Package Registry, and fallback catalogs together when a dependency is truly required.
- Do not create local copies of shared helpers.
- Keep commits focused and report exactly what changed and what was validated.

