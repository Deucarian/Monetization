# Rewarded Claim Primer

This pure C# sample records a rewarded claim once and rejects a duplicate claim. It demonstrates the package's SDK-free claim identity and ledger primitives; a product-owned provider adapter remains responsible for showing the actual ad.

Call `RewardedClaimPrimer.RecordExactlyOnce()` from an EditMode test or application bootstrap. A `true` result confirms the same claim cannot be granted twice.
