using MailingListChallenge;

namespace MailingListChallenge.Tests;

public sealed class MailingListBuilderTests
{
    private static ChallengeData LoadData()
    {
        var root = FindRepositoryRoot();
        return TestDataLoader.Load(Path.Combine(root, "test-data"));
    }

    [Fact]
    public async Task OldSystem_CurrentlyHasKnownBug_AndMissesZeroBasketDiscountedPurchase()
    {
        var data = LoadData();

        var builder = new OldMailingListBuilder(
            new InMemoryMemberBulkSource(data.Members),
            new InMemoryMemberTransactionStream(data.Transactions),
            new InMemoryMarketingFilterStream(data.MarketingFilters));

        var emails = await builder.BuildMarketingListAsync();

        Assert.Contains("alice@test.com", emails);
        Assert.DoesNotContain("bob@test.com", emails);
    }

    [Fact]
    public async Task NewSystem_ShouldMatchExpectedEmails_AfterCandidateImplementsIt()
    {
        var data = LoadData();

        var builder = new NewMailingListBuilder(
            new InMemoryMemberTransactionStream(data.Transactions),
            new InMemoryMemberApiClient(data.Members),
            new InMemoryMarketingFilterApiClient(data.MarketingFilters),
            clientId: 888);

        var emails = await builder.BuildMarketingListAsync();

        Assert.Equal(
            data.ExpectedEmails.OrderBy(x => x, StringComparer.OrdinalIgnoreCase),
            emails.OrderBy(x => x, StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task NewSystem_ShouldReturnDistinctEmails_WhenMemberHasMultipleQualifyingTransactions()
    {
        var data = LoadData();

        var builder = new NewMailingListBuilder(
            new InMemoryMemberTransactionStream(data.Transactions),
            new InMemoryMemberApiClient(data.Members),
            new InMemoryMarketingFilterApiClient(data.MarketingFilters),
            clientId: 888);

        var emails = await builder.BuildMarketingListAsync();

        Assert.Equal(emails.Count, emails.Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "test-data")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate repository root containing test-data.");
    }
}
