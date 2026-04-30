namespace MailingListChallenge;

public sealed record MemberRecord(
    int ClientId,
    int MemberId,
    string Email,
    string LastLoginDate);

public sealed record MemberTransaction(
    string TransactionId,
    int MemberId,
    string ProductName,
    string ProductCategory,
    decimal TotalBasketValue,
    decimal TotalDiscountDollarAmount);

public sealed record MarketingFilter(
    int ClientId,
    string ProductCategory,
    decimal? MinTotalBasketValue,
    decimal MinDiscountAmount);
