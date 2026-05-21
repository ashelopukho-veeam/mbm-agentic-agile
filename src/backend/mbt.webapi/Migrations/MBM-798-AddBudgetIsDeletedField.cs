using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using MongoDB.Driver;
using MongoDBMigrations;
using Budget = mbt.webapi.Domain.Entities.Budget;
using Version = MongoDBMigrations.Version;

namespace mbt.webapi.Migrations;

[UsedImplicitly]
public class Mbm798AddBudgetIsDeletedField : IMigration
{
    public void Up(IMongoDatabase database)
    {
        var collection = database.GetCollection<Budget>(CollectionNames.Budgets);

        var updBuilder = new UpdateDefinitionBuilder<Budget>();
        var upd = updBuilder
            .Set(b => b.IsDeleted, false);

        collection.UpdateMany(_ => true, upd);
    }


    public void Down(IMongoDatabase database)
    {
    }

    public Version Version => new(1, 4, 0);

    public string Name => "Mbm798AddBudgetIsDeletedField";
}
