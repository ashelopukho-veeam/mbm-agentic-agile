using System;
using System.Collections.Generic;
using mbt.webapi.BuiltIn;
using mbt.webapi.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Budget = mbt.webapi.Domain.Entities.Budget;
using DictionaryDocument = mbt.webapi.Domain.Entities.DictionaryDocument;

namespace mbt.webapi.Migrations;

public static class DbSeeder
{
    public static void SeedDatabase(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var dictionaryRepository = services.GetRequiredService<IDbBaseRepository<DictionaryDocument>>();
            InitDictionaries(dictionaryRepository);
        }
        catch (Exception)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError("An error occurred while seeding the database");
        }
    }


    private static void InitDictionaries(IDbBaseRepository<DictionaryDocument> dictionaryRepository)
    {
        if (dictionaryRepository.Count() != 0) return;

        var segmentsDictionary = new DictionaryDocument(BuiltInDictionariesInternalNames.Segments)
        {
            Items = new List<string>()
            {
                "COM",
                "SMB",
                "FED",
                "SLED",
                "VCSP",
                "ENT"
            }
        };
        dictionaryRepository.Create(segmentsDictionary);

        var campaignsDictionary = new DictionaryDocument(BuiltInDictionariesInternalNames.Campaigns)
        {
            Items = new List<string>()
            {
                "Infrastructure & non-campaign related",
                "Thought Leadership - Thought Leadership",
                "Market Forces - Ransomware",
                "Market Forces - Hybrid Cloud & IT",
                "High Velocity - B&R",
                "High Velocity - M365",
                "New Markets - AWS",
                "New Markets - Azure",
                "New Markets - Google",
                "New Markets - Salesforce",
                "New Markets - Kasten",
                "High Touch - Enterprise/Upper Commercial",
                "Partner - VCSP ProPartner",
                "Partner - Reseller ProPartner",
                "Partner - Alliance - HPE",
                "Partner - Alliance - Cisco",
                "Partner - Alliance - NetApp",
                "Partner - Alliance - Pure Storage",
                "Partner - Alliance - Lenovo",
                "Partner - Alliances - Other",
                "Renewals - Renewals & Customer Lifecycle"
            }
        };
        dictionaryRepository.Create(campaignsDictionary);

        var executionTeamDictionary = new DictionaryDocument("Execution Team")
        {
            InternalName = "execution_team",
            Items = new List<string>()
            {
                "Unspecified",
                "Effective Spend",
                "JellyFish",
                "Veeam SMM Team",
                "Veeam Digital Advertising Team",
                "DWA Media",
                "Le Grand",
                "Content Marketing",
                "Digital Marketing Other",
                "Social Media",
                "Web Marketing",
                "Agency fees (global)",
                "Global reserve",
                "Campaign Marketing",
                "Yinn Comms"
            }
        };
        dictionaryRepository.Create(executionTeamDictionary);

        var contentTypeDictionary = new DictionaryDocument("Content Type")
        {
            InternalName = "content_type",
            Items = new List<string>()
            {
                "Unspecified",
                "trial",
                "free",
                "content",
                "demo",
                "promo",
                "other",
                "general",
                "event",
                "BANT",
                "video"
            }
        };
        dictionaryRepository.Create(contentTypeDictionary);

        var managementDictionary = new DictionaryDocument("Management")
        {
            Items = new List<string>()
            {
                "Unspecified", "Agency", "SMM team", "Veeam Team"
            }
        };
        dictionaryRepository.Create(managementDictionary);

        var alliancesDictionary = new DictionaryDocument("Alliances")
        {
            Items = new List<string>()
            {
                "HPE",
                "Cisco",
                "NetApp",
                "Lenovo",
                "Nutanix",
                "VMware",
                "Microsoft",
                "Pure Storage",
                "GSI",
                "Fujitsu",
                "Red Hat",
                "Hitachi",
                "Google",
                "AWS",
                "Huawei",
                "Wasabi",
                "IBM",
                "ExaGrid",
                "Dell/EMC"
            }
        };
        dictionaryRepository.Create(alliancesDictionary);

        var channelDetailsDictionary = new DictionaryDocument("Channel Details")
        {
            InternalName = "channel_details",
            Items = new List<string>()
            {
                "Distributors",
                "Tier 1",
                "Tier 2",
                "Aggregators"
            }
        };
        dictionaryRepository.Create(channelDetailsDictionary);

        var budgetTypesDictionary = new DictionaryDocument("Budget Types")
        {
            InternalName = BuiltInDictionariesInternalNames.BudgetTypes,
            Items = new List<string>()
            {
                "Corporate Marketing",
                "Field Marketing",
                "Sales",
                "Kasten"
            }
        };
        dictionaryRepository.Create(budgetTypesDictionary);
    }

    public static void CreateIndexAsync(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        var collection = database.GetCollection<Budget>(CollectionNames.Budgets);

        // clear all indexes
        collection.Indexes.DropAll();

        // create partial index mongodb (for soft delete support)
        var filter = Builders<Budget>.Filter.Eq(b => b.IsDeleted, false);
        var options = new CreateIndexOptions<Budget> { Unique = true, PartialFilterExpression = filter };
        var indexDefinition = Builders<Budget>.IndexKeys.Ascending(b => b.Title);

        var createIndexModel = new CreateIndexModel<Budget>(indexDefinition, options);
        collection.Indexes.CreateOne(createIndexModel);
    }
}
