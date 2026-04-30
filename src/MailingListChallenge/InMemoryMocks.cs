namespace MailingListChallenge;

public sealed class InMemoryMemberTransactionStream : IMemberTransactionStream
{
    private readonly IReadOnlyList<MemberTransaction> _transactions;

    public InMemoryMemberTransactionStream(IReadOnlyList<MemberTransaction> transactions)
    {
        _transactions = transactions;
    }

    public async IAsyncEnumerable<MemberTransaction> ReadTransactionsAsync()
    {
        foreach (var transaction in _transactions)
        {
            await Task.Yield();
            yield return transaction;
        }
    }
}

public sealed class InMemoryMemberBulkSource : IMemberBulkSource
{
    private readonly IReadOnlyList<MemberRecord> _members;

    public InMemoryMemberBulkSource(IReadOnlyList<MemberRecord> members)
    {
        _members = members;
    }

    public Task<IReadOnlyList<MemberRecord>> GetAllMembersAsync() => Task.FromResult(_members);
}

public sealed class InMemoryMarketingFilterStream : IMarketingFilterStream
{
    private readonly IReadOnlyList<MarketingFilter> _filters;

    public InMemoryMarketingFilterStream(IReadOnlyList<MarketingFilter> filters)
    {
        _filters = filters;
    }

    public async IAsyncEnumerable<MarketingFilter> ReadFiltersAsync()
    {
        foreach (var filter in _filters)
        {
            await Task.Yield();
            yield return filter;
        }
    }
}

public sealed class InMemoryMemberApiClient : IMemberApiClient
{
    private readonly IReadOnlyList<MemberRecord> _members;

    public InMemoryMemberApiClient(IReadOnlyList<MemberRecord> members)
    {
        _members = members;
    }

    public Task<IReadOnlyList<MemberRecord>> GetMembersAsync(IReadOnlyCollection<int> memberIds)
    {
        var idSet = memberIds.ToHashSet();
        IReadOnlyList<MemberRecord> result = _members
            .Where(member => idSet.Contains(member.MemberId))
            .ToList();

        return Task.FromResult(result);
    }
}

public sealed class InMemoryMarketingFilterApiClient : IMarketingFilterApiClient
{
    private readonly IReadOnlyList<MarketingFilter> _filters;

    public InMemoryMarketingFilterApiClient(IReadOnlyList<MarketingFilter> filters)
    {
        _filters = filters;
    }

    public Task<IReadOnlyList<MarketingFilter>> GetMarketingFiltersAsync(int clientId)
    {
        IReadOnlyList<MarketingFilter> result = _filters
            .Where(filter => filter.ClientId == clientId)
            .ToList();

        return Task.FromResult(result);
    }
}
