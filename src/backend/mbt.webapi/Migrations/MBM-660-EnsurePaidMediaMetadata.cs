using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBMigrations;
using Newtonsoft.Json;
using TreeItem = mbt.webapi.Domain.Entities.TreeItem;

namespace mbt.webapi.Migrations;

[UsedImplicitly]
public class Mbm660EnsurePaidMediaMetadata : IMigration
{
    public void Up(IMongoDatabase database)
    {
        var jsonText = File.ReadAllText("Migrations/Data/PaidMediaMetadata.json");
        var paidMediaData = JsonConvert.DeserializeObject<MetaTreeItem>(jsonText);


        if (paidMediaData == null) return;

        var collection = database.GetCollection<TreeItem>(CollectionNames.Metadata);

        var checkedPaidMediaTreeItem = collection.FindSync(t => t.Title == paidMediaData.Title).FirstOrDefault();
        if (checkedPaidMediaTreeItem != null) return;

        var root = new TreeItem()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = paidMediaData.Title,
            Value = paidMediaData.Title
        };
        collection.InsertOne(root);
        AddChildren(collection, root, paidMediaData.Children);
    }

    private void AddChildren(IMongoCollection<TreeItem> collection, TreeItem parent, List<MetaTreeItem> children)
    {
        if (children == null || children.Count == 0)
            return;

        foreach (var metaChild in children)
        {
            var child = new TreeItem()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Title = metaChild.Title,
                Value = metaChild.Title,
                ParentId = parent.Id
            };
            collection.InsertOne(child);
            AddChildren(collection, child, metaChild.Children);
        }
    }


    public void Down(IMongoDatabase database)
    {
    }

    public Version Version => new(1, 2, 0);

    public string Name => "Mbm660EnsurePaidMediaMetadata";
}

[UsedImplicitly]
internal class MetaTreeItem
{
    public string Title { get; set; }
    public List<MetaTreeItem> Children { get; set; }
}
