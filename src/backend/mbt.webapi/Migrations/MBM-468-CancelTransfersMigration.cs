using System;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using MongoDB.Driver;
using MongoDBMigrations;
using Transfer = mbt.webapi.Domain.Entities.Transfer;
using TransferStatus = mbt.webapi.Domain.Entities.TransferStatus;
using Version = MongoDBMigrations.Version;

namespace mbt.webapi.Migrations;

[UsedImplicitly]
// ReSharper disable once InconsistentNaming
public class MBM468CancelTransfersMigration : IMigration
{
    public Version Version => new(1, 1, 0);

    public string Name => "MBM-468-CancelTransfersMigration";

    public void Up(IMongoDatabase database)

    {
        var transfers = database.GetCollection<Transfer>(CollectionNames.Transfers);

        foreach (var transferId in _transfersIDsToCancel)
        {
            var updateDefinitionBuilder = new UpdateDefinitionBuilder<Transfer>();
            var update = updateDefinitionBuilder
                .Set(t => t.Status, TransferStatus.Canceled)
                .Set(t => t.Modified, DateTime.Now);
            transfers.UpdateOne(t => t.Id == transferId, update);
        }
    }

    public void Down(IMongoDatabase database)
    {
// ...
    }

    private readonly string[] _transfersIDsToCancel =
    {
        "623c73a7112186646483a221",
        "623c73751121866464839c51",
        "623c732d1121866464839681",
        "623c72e011218664648390b1",
        "623c72881121866464838031",
        "623c719011218664648374f9",
        "623c65628913bc55990ac768",
        "623c65208913bc55990ac198",
        "623c64a98913bc55990abbc8",
        "623c640b8913bc55990ab5f8",
        "623c635e8913bc55990ab028",
        "623c62bc8913bc55990aaa60",
        "623c628a8913bc55990aa490"
    };
}
