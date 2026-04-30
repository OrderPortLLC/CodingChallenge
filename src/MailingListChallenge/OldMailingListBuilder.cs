namespace MailingListChallenge;

public sealed class OldMailingListBuilder
{
    private readonly IMemberBulkSource _memberBulkSource;
    private readonly IMemberTransactionStream _transactionStream;
    private readonly IMarketingFilterStream _filterStream;

    public OldMailingListBuilder(
        IMemberBulkSource memberBulkSource,
        IMemberTransactionStream transactionStream,
        IMarketingFilterStream filterStream)
    {
        _memberBulkSource = memberBulkSource;
        _transactionStream = transactionStream;
        _filterStream = filterStream;
    }

    public async Task<IReadOnlyList<string>> BuildMarketingListAsync()
    {
        var members = await _memberBulkSource.GetAllMembersAsync();
        var transactions = new List<MemberTransaction>();
        var filters = new List<MarketingFilter>();

        await foreach (var transaction in _transactionStream.ReadTransactionsAsync())
        {
            transactions.Add(transaction);
        }

        await foreach (var filter in _filterStream.ReadFiltersAsync())
        {
            filters.Add(filter);
        }

        return BuildEmails(members, transactions, filters);
    }

    private static IReadOnlyList<string> BuildEmails(
        IReadOnlyList<MemberRecord> members,
        IReadOnlyList<MemberTransaction> transactions,
        IReadOnlyList<MarketingFilter> filters)
    {
        var emails =
            from member in members
            join transaction in transactions on member.MemberId equals transaction.MemberId
            from filter in filters
            where member.ClientId == filter.ClientId
            where transaction.ProductCategory == filter.ProductCategory
            where transaction.TotalDiscountDollarAmount > filter.MinDiscountAmount
            // TODO: Bug hint for candidate: minTotalBasketValue = null should mean "match any basket value".
            // The current code treats null like 0 and requires TotalBasketValue > 0.
            // This excludes valid discounted transactions where TotalBasketValue is 0.
            where filter.MinTotalBasketValue == null
                ? transaction.TotalBasketValue > 0
                : transaction.TotalBasketValue >= filter.MinTotalBasketValue.Value
            select member.Email;

        return emails
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(email => email, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
