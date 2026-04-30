# Interviewer Notes

## Intended Solution Shape

A strong candidate should implement `NewMailingListBuilder.BuildMarketingListAsync()` roughly as follows:

1. Read transactions from the async stream into a list.
2. Extract distinct member IDs.
3. Call `GetMembersAsync(memberIds)` once.
4. Call `GetMarketingFiltersAsync(_clientId)` once.
5. Join members to transactions by `MemberId`.
6. Apply filters using member `ClientId` because transactions do not contain `ClientId`.
7. Treat `MinTotalBasketValue == null` as no basket-value constraint.
8. Return distinct sorted emails.

## Discussion Prompts

- Why does client matching have to come from `MemberRecord`?
- What are the risks of replacing a push model with polling?
- How would this change if the member API had a max batch size?
- How would this change if transaction volume were large?
- Where would you put retry, backoff, cancellation, and observability in production?
- What would you test if this were a real migration?

## Expected Final Output

```text
alice@test.com
bob@test.com
```

## Intentional Edge Cases

- `alice@test.com` has two qualifying transactions but should appear once.
- `bob@test.com` has `TotalBasketValue = 0.00` but should match because the basket filter is null.
- `wrong-client@test.com` has a qualifying transaction but the wrong client ID.
- `red-wine@test.com` has the wrong product category for the target filter.
- `no-discount@test.com` has insufficient discount.
- `carol@test.com` has discount exactly equal to the threshold and should not match because the rule is strictly greater than.
