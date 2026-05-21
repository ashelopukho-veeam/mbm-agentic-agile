using System;
using System.Collections.Generic;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration;
using mbt.webapi.Domain.Entities;
using MongoDB.Driver;

namespace mbt.webapi.Repositories;

public interface IDbContext
{
    IMongoCollection<T> GetCollection<T>();
}

public class DbContext : IDbContext
{
    private readonly IMongoDatabase _database;

    public DbContext(IMbtDatabaseSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.DatabaseName);

        _keyValuePairs.Add(typeof(MailEntity), CollectionNames.Mail);
        _keyValuePairs.Add(typeof(Budget), CollectionNames.Budgets);
        _keyValuePairs.Add(typeof(DictionaryDocument), CollectionNames.Dictionaries);
        _keyValuePairs.Add(typeof(Transfer), CollectionNames.Transfers);
        _keyValuePairs.Add(typeof(NotificationEvent), CollectionNames.NotificationEvents);
        _keyValuePairs.Add(typeof(GroupedActivity), CollectionNames.GroupedActivities);
        _keyValuePairs.Add(typeof(IncrementalFund), CollectionNames.IncrementalFunds);
        _keyValuePairs.Add(typeof(TreeItem), CollectionNames.Metadata);
        _keyValuePairs.Add(typeof(PaidMediaSet), CollectionNames.PaidMediaSet);
        _keyValuePairs.Add(typeof(UserProfile), CollectionNames.Users);
        _keyValuePairs.Add(typeof(VendorsDocument), CollectionNames.Vendors);
        _keyValuePairs.Add(typeof(Chat), CollectionNames.Chat);
        _keyValuePairs.Add(typeof(PaidMediaTeamApprover), CollectionNames.PaidMediaTeamApprovers);
        _keyValuePairs.Add(typeof(CostCenter), CollectionNames.CostCenters);
        _keyValuePairs.Add(typeof(BudgetLevel), CollectionNames.Levels);
        _keyValuePairs.Add(typeof(AppConfig), CollectionNames.AppConfig);
        _keyValuePairs.Add(typeof(CommonConfig), CollectionNames.CommonConfig);
        _keyValuePairs.Add(typeof(BudgetPlanConfig), CollectionNames.BudgetPlanConfig);
        _keyValuePairs.Add(typeof(MbtTask), CollectionNames.Tasks);
        _keyValuePairs.Add(typeof(BudgetPlanHistoryItem), CollectionNames.BudgetPlanHistory);
        _keyValuePairs.Add(typeof(IncrementalFundsConfig), CollectionNames.IncrementalFundsConfig);
        _keyValuePairs.Add(typeof(CurrencyRate), CollectionNames.CurrencyRates);
        _keyValuePairs.Add(typeof(WorkflowEntity), CollectionNames.Workflows);

    }

    private readonly Dictionary<Type, string> _keyValuePairs = new();

    public IMongoCollection<T> GetCollection<T>()
    {
        if (!_keyValuePairs.ContainsKey(typeof(T)))
        {
            throw new Exception($"Collection for type {typeof(T).Name} not found");
        }

        return _database.GetCollection<T>(_keyValuePairs[typeof(T)]);
    }
}

