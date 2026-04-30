namespace MailingListChallenge;

public interface IMemberTransactionStream
{
    IAsyncEnumerable<MemberTransaction> ReadTransactionsAsync();
}

public interface IMemberBulkSource
{
    Task<IReadOnlyList<MemberRecord>> GetAllMembersAsync();
}

public interface IMarketingFilterStream
{
    IAsyncEnumerable<MarketingFilter> ReadFiltersAsync();
}

public interface IMemberApiClient
{
    Task<IReadOnlyList<MemberRecord>> GetMembersAsync(IReadOnlyCollection<int> memberIds);
}

public interface IMarketingFilterApiClient
{
    Task<IReadOnlyList<MarketingFilter>> GetMarketingFiltersAsync(int clientId);
}
