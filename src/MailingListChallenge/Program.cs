namespace MailingListChallenge;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var dataDirectory = args.Length > 0
            ? args[0]
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "test-data"));

        var data = TestDataLoader.Load(dataDirectory);

        var oldBuilder = new OldMailingListBuilder(
            new InMemoryMemberBulkSource(data.Members),
            new InMemoryMemberTransactionStream(data.Transactions),
            new InMemoryMarketingFilterStream(data.MarketingFilters));

        var emails = await oldBuilder.BuildMarketingListAsync();

        Console.WriteLine("Old system output:");
        foreach (var email in emails)
        {
            Console.WriteLine(email);
        }
    }
}
