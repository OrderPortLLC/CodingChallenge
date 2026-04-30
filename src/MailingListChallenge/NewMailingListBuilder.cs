namespace MailingListChallenge;

public sealed class NewMailingListBuilder
{
    private readonly IMemberTransactionStream _transactionStream;
    private readonly IMemberApiClient _memberApiClient;
    private readonly IMarketingFilterApiClient _marketingFilterApiClient;
    private readonly int _clientId;

    public NewMailingListBuilder(
        IMemberTransactionStream transactionStream,
        IMemberApiClient memberApiClient,
        IMarketingFilterApiClient marketingFilterApiClient,
        int clientId)
    {
        _transactionStream = transactionStream;
        _memberApiClient = memberApiClient;
        _marketingFilterApiClient = marketingFilterApiClient;
        _clientId = clientId;
    }

    public async Task<IReadOnlyList<string>> BuildMarketingListAsync()
    {
        // Candidate task:
        // 1. Read transactions from the transaction stream.
        // 2. Collect distinct impacted member IDs.
        // 3. Fetch impacted members in one batched API call.
        // 4. Fetch marketing filters for the configured client ID.
        // 5. Combine transactions, members, and filters.
        // 6. Return distinct matching emails.
        throw new NotImplementedException("Refactor the old implementation to use the new polling-based inputs.");
    }
}
