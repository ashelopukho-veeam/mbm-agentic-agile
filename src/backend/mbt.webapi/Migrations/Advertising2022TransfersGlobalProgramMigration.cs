using System;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Utils;
using MongoDB.Driver;
using MongoDBMigrations;
using Transfer = mbt.webapi.Domain.Entities.Transfer;
using Version = MongoDBMigrations.Version;

namespace mbt.webapi.Migrations;

[UsedImplicitly]
public class Advertising2022TransfersGlobalProgramMigration : IMigration
{
    public Version Version => new(1, 0, 0);

    public string Name => "Advertising_2022_Transfers_to fix _Program_value";

    public void Up(IMongoDatabase database)

    {
        var migrationData = CsvUtils<MigrationItem>.GetCsvDataFromFile(
            "Migrations/Data/Advertising_2022_Transfers_to fix _Program_value.csv");

        var transfers = database.GetCollection<Transfer>(CollectionNames.Transfers);

        foreach (var migrationItem in migrationData)
        {
            var updateDefinitionBuilder = new UpdateDefinitionBuilder<Transfer>();
            var update = updateDefinitionBuilder.Set(t => t.GlobalProgram, migrationItem.GlobalProgram)
                .Set(t => t.Modified, DateTime.Now);
            transfers.UpdateOne(t => t.Id == migrationItem.Id, update);
        }
    }

    public void Down(IMongoDatabase database)

    {
// ...
    }

    [PublicAPI]
    private class MigrationItem
    {
        public string Id { get; set; }
        public string GlobalProgram { get; set; }
    }
}
