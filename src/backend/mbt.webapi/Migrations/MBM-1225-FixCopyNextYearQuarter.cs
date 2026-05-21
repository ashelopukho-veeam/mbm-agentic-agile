using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using MongoDB.Driver;
using MongoDBMigrations;
using Budget = mbt.webapi.Domain.Entities.Budget;
using Version = MongoDBMigrations.Version;

namespace mbt.webapi.Migrations;

[UsedImplicitly]
public class Mbm1225FixCopyNextYearQuarter : IMigration
{
    public void Up(IMongoDatabase database)
    {
        var collection = database.GetCollection<Budget>(CollectionNames.Budgets);

        var updBuilder = new UpdateDefinitionBuilder<Budget>();
        var upd = updBuilder
            .Set(b => b.Plans[3].Quarter, "Q4");

        collection.UpdateMany(_ => true, upd);
    }


    public void Down(IMongoDatabase database)
    {
    }

    public Version Version => new(1, 6, 0);

    public string Name => nameof(Mbm1225FixCopyNextYearQuarter);
}
