# NewMailingListBuilder – Implementation Specification

## Overview

`NewMailingListBuilder.BuildMarketingListAsync()` builds a list of **distinct email addresses** for members who match one or more marketing filters.

This implementation replaces the old push-based system with a **polling-based workflow** while preserving identical output behavior.

---

## Method Contract

```csharp
Task<IReadOnlyList<string>> BuildMarketingListAsync()
```

### Returns

* A list of **distinct email addresses**
* Order is not important
* Must match the output of the old system

---

## Data Sources

### 1. Member Transactions (WebSocket)

```csharp
IAsyncEnumerable<MemberTransaction>
```

Fields:

* `transactionId`
* `memberId`
* `productCategory`
* `totalBasketValue`
* `totalDiscountDollarAmount`

### 2. Member API (GET)

```csharp
Task<IReadOnlyList<MemberRecord>> GetMembersAsync(IReadOnlyCollection<int> memberIds)
```

Fields:

* `clientId`
* `memberId`
* `email`
* `lastLoginDate` (not used)

### 3. Marketing Filters API (GET)

```csharp
Task<IReadOnlyList<MarketingFilter>> GetMarketingFiltersAsync(int clientId)
```

Fields:

* `clientId`
* `productCategory`
* `minTotalBasketValue`
* `minDiscountAmount`

---

## Processing Flow

### Step 1: Read Transactions

* Read all transactions from the websocket stream
* Materialize into memory (list)

```csharp
var transactions = await stream.ReadTransactionsAsync().ToListAsync();
```

---

### Step 2: Collect Impacted Member IDs

* Extract all unique `memberId` values from transactions

```csharp
var memberIds = transactions
    .Select(t => t.MemberId)
    .Distinct()
    .ToList();
```

---

### Step 3: Fetch Members

* Call the member API once with all member IDs

```csharp
var members = await memberApi.GetMembersAsync(memberIds);
```

* Build lookup:

```csharp
var membersById = members.ToDictionary(m => m.MemberId);
```

---

### Step 4: Determine Client Scope

* Identify all `clientId` values from members

```csharp
var clientIds = members
    .Select(m => m.ClientId)
    .Distinct();
```

---

### Step 5: Fetch Marketing Filters

* For each `clientId`, fetch filters

```csharp
var filters = new List<MarketingFilter>();

foreach (var clientId in clientIds)
{
    var clientFilters = await filterApi.GetMarketingFiltersAsync(clientId);
    filters.AddRange(clientFilters);
}
```

---

### Step 6: Apply Filtering Logic

A transaction qualifies if **all conditions are met**:

```text
member.clientId == filter.clientId
transaction.productCategory == filter.productCategory
transaction.totalDiscountDollarAmount > filter.minDiscountAmount
```

#### Basket Value Rule

* If `filter.minTotalBasketValue != null`:

```text
transaction.totalBasketValue >= filter.minTotalBasketValue
```

* If `filter.minTotalBasketValue == null`:

  * **Do not filter on basket value**

---

### Step 7: Join Data

For each transaction:

1. Find matching member
2. Evaluate against all filters
3. If any filter matches → include member email

---

### Step 8: Deduplicate Emails

Exercise Left to developer

---

## Important Rules

### 1. Client Matching

Transactions do NOT contain `clientId`.

Client matching must come from:

```csharp
member.clientId == filter.clientId
```

---

### 2. Discount Rule

Strictly greater than:

```csharp
transaction.totalDiscountDollarAmount > filter.minDiscountAmount
```

---

### 3. Null Basket Rule

```text
null = no filtering
```

Do NOT treat null as zero.

---

### 4. Multiple Filters

* Multiple filters may exist
* A transaction matching ANY filter qualifies the member

---

### 5. Distinct Emails Only

* Final output must not contain duplicates

---

## Expected Output Behavior

The result must:

* Match the old system output exactly
* Pass all unit tests
* Be deterministic given the same input data

---

## Success Criteria

Implementation is complete when:

* All unit tests pass
* Output matches old system
* Code is readable and maintainable
* Logic is clearly separated (optional helper methods encouraged)

