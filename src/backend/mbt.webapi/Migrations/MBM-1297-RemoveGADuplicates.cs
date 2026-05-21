using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBMigrations;
using Version = MongoDBMigrations.Version;

namespace mbt.webapi.Migrations;

[UsedImplicitly]
public class Mbm1297RemoveGaDuplicates : IMigration
{
    public void Up(IMongoDatabase database)
    {
        var collection = database.GetCollection<GroupedActivity>(CollectionNames.GroupedActivities);

        var aggregation = new[]
        {
            new BsonDocument("$group",
                new BsonDocument
                {
                    {
                        "_id",
                        new BsonDocument
                        {
                            { "title", "$Title" },
                            { "budgetPlan", "$BudgetPlanId" },
                            { "budget", "$BudgetId" }
                        }
                    },
                    {
                        "count",
                        new BsonDocument("$sum", 1)
                    },
                    {
                        "ga",
                        new BsonDocument("$push",
                            new BsonDocument
                            {
                                { "Idx", "$$ROOT._id" },
                                { "Created", "$$ROOT.Created" }
                            })
                    }
                }),
            new BsonDocument("$match",
                new BsonDocument("count",
                    new BsonDocument("$gt", 1))),
            new BsonDocument("$project",
                new BsonDocument
                {
                    {
                        "Result",
                        new BsonDocument("$sortArray",
                            new BsonDocument
                            {
                                { "input", "$ga" },
                                {
                                    "sortBy",
                                    new BsonDocument("Created", -1)
                                }
                            })
                    },
                    { "_id", 0 }
                })
        };

        var result = collection.Aggregate<AggregationResult>(aggregation).ToList();

        foreach (var item in result)
        {
            var idxs = item.Result.Select(x => x.Idx).Skip(1).Select(t => t.ToString()).ToList();
            collection.DeleteMany(x => idxs.Contains(x.Id));
        }
    }


    public void Down(IMongoDatabase database)
    {
    }

    public Version Version => new(1, 7, 0);
    public string Name => nameof(Mbm1297RemoveGaDuplicates);
}

public class AggregationResult
{
    public List<ResultItem> Result { get; set; }
}
public class ResultItem
{
    public ObjectId Idx { get; set; }
    public DateTime Created { get; set; }
}
