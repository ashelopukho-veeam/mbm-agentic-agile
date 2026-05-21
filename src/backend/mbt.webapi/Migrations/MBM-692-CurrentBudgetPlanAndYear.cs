using System;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Extensions;
using MongoDB.Driver;
using MongoDBMigrations;
using BudgetPlanConfig = mbt.webapi.Domain.Entities.BudgetPlanConfig;
using Version = MongoDBMigrations.Version;

namespace mbt.webapi.Migrations;

[UsedImplicitly]
public class Mbm692CurrentBudgetPlanAndYear : IMigration
{
    public void Up(IMongoDatabase database)
    {
        var collection = database.GetCollection<BudgetPlanConfig>(CollectionNames.BudgetPlanConfig);

        var config = collection.Find(_ => true).FirstOrDefault() ?? new BudgetPlanConfig();

        var currentQuarter = DateTime.Now.GetQuarter();
        if (currentQuarter == 4)
        {
            config.CurrentBudgetPlanYear = DateTime.Now.Year + 1;
            config.CurrentBudgetPlanName = "Q1";
        }
        else
        {
            config.CurrentBudgetPlanYear = DateTime.Now.Year;
            config.CurrentBudgetPlanName = "Q" + currentQuarter;
        }


        collection.ReplaceOne(c => c.Id == config.Id, config, new ReplaceOptions() { IsUpsert = true });
    }


    public void Down(IMongoDatabase database)
    {
    }

    public Version Version => new(1, 3, 0);

    public string Name => "Mbm692CurrentBudgetPlanAndYear";
}
