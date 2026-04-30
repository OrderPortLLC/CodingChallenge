# C# Pair Programming Challenge: Mailing List Refactor

## Premise

A set of services builds marketing mailing lists by combining member records, member transaction events, and marketing filters.

The old implementation is push-based:

1. Bulk POST of all members.
2. WebSocket stream of member transactions.
3. WebSocket stream of marketing filters.

The new implementation keeps the transaction WebSocket, but replaces the member and marketing-filter pushes with GET-style polling.

Your task is to refactor the code so the new system produces the same mailing list output as the corrected old behavior.

## Runtime

This challenge is intentionally small:

- C# / .NET 8
- Console app
- xUnit tests
- In-memory mocks only
- No real HTTP server
- No real WebSocket server
- No timers

## Candidate Task

Implement `NewMailingListBuilder.BuildMarketingListAsync()`.

The method should:

1. Read all emitted transactions from `IMemberTransactionStream`.
2. Collect distinct impacted `memberId` values.
3. Fetch impacted members in one batched call using `IMemberApiClient.GetMembersAsync(...)`.
4. Fetch marketing filters for the configured client using `IMarketingFilterApiClient.GetMarketingFiltersAsync(clientId)`.
5. Combine transactions, members, and filters.
6. Return distinct matching email addresses.

## Matching Rules

A member email qualifies when:

```text
member.clientId == filter.clientId
transaction.memberId == member.memberId
transaction.productCategory == filter.productCategory
transaction.totalDiscountDollarAmount > filter.minDiscountAmount
```

If `filter.minTotalBasketValue` is not null, also require:

```text
transaction.totalBasketValue >= filter.minTotalBasketValue
```

If `filter.minTotalBasketValue` is null, match any basket value.

The final email list must be distinct. A member with multiple qualifying transactions should only appear once.

## Known Bug in Old System

The old implementation has an intentional bug. It treats `minTotalBasketValue = null` as if it means `TotalBasketValue > 0`.

Correct behavior: null means do not apply any basket-value filter.

The bug is intentionally easy to find and marked with a `TODO` comment in `OldMailingListBuilder.cs`.

## Test Data

Test data lives in `/test-data`:

- `members.json`
- `transactions.json`
- `marketing-filters.json`
- `expected-emails.json`

Expected output:

```text
alice@test.com
bob@test.com
```

`bob@test.com` is the important edge case. Bob has a qualifying discount, but a basket value of `0.00`. This should still match because `minTotalBasketValue` is null.

## Suggested Commands

```bash
dotnet restore
dotnet test
```

Optional console run:

```bash
dotnet run --project src/MailingListChallenge/MailingListChallenge.csproj
```

## Interview Guidance

This challenge should be run as a pair-programming exercise. The candidate should be encouraged to talk through:

- What behavior must be preserved?
- What changed between push and polling?
- Where should batching happen?
- How should null filters be interpreted?
- How should duplicate qualifying transactions be handled?
- What tests should be added before or after the refactor?
