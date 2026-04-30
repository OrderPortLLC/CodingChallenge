using System.Text.Json;

namespace MailingListChallenge;

public sealed record ChallengeData(
    IReadOnlyList<MemberRecord> Members,
    IReadOnlyList<MemberTransaction> Transactions,
    IReadOnlyList<MarketingFilter> MarketingFilters,
    IReadOnlyList<string> ExpectedEmails);

public static class TestDataLoader
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static ChallengeData Load(string dataDirectory)
    {
        return new ChallengeData(
            ReadJson<List<MemberRecord>>(Path.Combine(dataDirectory, "members.json")),
            ReadJson<List<MemberTransaction>>(Path.Combine(dataDirectory, "transactions.json")),
            ReadJson<List<MarketingFilter>>(Path.Combine(dataDirectory, "marketing-filters.json")),
            ReadJson<List<string>>(Path.Combine(dataDirectory, "expected-emails.json")));
    }

    private static T ReadJson<T>(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json, Options)
            ?? throw new InvalidOperationException($"Could not deserialize {path}");
    }
}
