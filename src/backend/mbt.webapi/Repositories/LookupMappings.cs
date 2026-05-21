using System.Collections.Generic;
using mbt.webapi.BuiltIn;

namespace mbt.webapi.Repositories;

public class MongoLookup
{
    public string CollectionName { get; set; }
    public string SourceField { get; set; }
    public string TargetField { get; set; }
    public string Alias { get; set; }

    public MongoLookup(string collectionName, string sourceField, string targetField, string alias)
    {
        CollectionName = collectionName;
        SourceField = sourceField;
        TargetField = targetField;
        Alias = alias;
    }
}

public static class LookupMappings
{
    public static List<MongoLookup
    > PaidMediaSetWithLinkedItemExpanded(string collectionName)
    {
        return new List<MongoLookup
        >
        {
            new(collectionName, "LinkedItemId", "_id", "LinkedItem"),
        };
    }

    public static readonly List<MongoLookup
    > GroupedActivityExpanded = new()
    {
        new MongoLookup
            (CollectionNames.Budgets, "BudgetId", "_id", "Budget"),
    };

    public static readonly List<MongoLookup
    > TaskExpandedMapping = new()
    {
        new MongoLookup
            (CollectionNames.Users, "AssignedTo", "_id", "AssignedToUser"),
    };

    public static readonly List<MongoLookup
    > PaidMediaTeamApproverExpandedMapping = new()
    {
        new MongoLookup
            (CollectionNames.Users, "ApproverId", "_id", "Approver")
    };

    public static readonly List<MongoLookup
    > LookupAuditableMapping = new()
    {
        new MongoLookup
            (CollectionNames.Users, "ModifiedBy", "_id", "ModifiedByUser"),
        new MongoLookup
            (CollectionNames.Users, "CreatedBy", "_id", "CreatedByUser"),
    };

    public static readonly List<MongoLookup
    > BudgetStructureUsersExpanded = new()
    {
        new MongoLookup
            (CollectionNames.Users, "OwnerId", "_id", "Owner"),
        new MongoLookup
            (CollectionNames.Users, "ParentManagerId", "_id", "ParentManager"),
        new MongoLookup
            (CollectionNames.Users, "ManagerId", "_id", "Manager"),
    };

    public static readonly List<MongoLookup
    > IncrementalFundsExpanded = new()
    {
        new MongoLookup
            (CollectionNames.Budgets, "ToBudgetId", "_id", "ToBudget"),
    };

    public static readonly List<MongoLookup
    > TransfersExpandedMapping = new()
    {
        new MongoLookup
            (CollectionNames.Budgets, "FromBudgetId", "_id", "FromBudget"),
        new MongoLookup
            (CollectionNames.Budgets, "ToBudgetId", "_id", "ToBudget"),
    };
}
