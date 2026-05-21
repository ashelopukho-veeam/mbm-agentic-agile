using System;
using mbt.webapi.WF;
using mbt.webapi.WF.CloneGroupedActivities;
using mbt.webapi.WF.IncrementalFunds;
using mbt.webapi.WF.IncrementalFunds.v2;
using mbt.webapi.WF.Steps;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using WorkflowCore.Interface;

namespace mbt.webapi.Configuration;

public static class WorkflowConfiguration
{
    public static void AddWorkflowCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddWorkflow(x =>
        {
            var dbConfigSection = configuration.GetRequiredSection(nameof(MbtDatabaseSettings));
            var dbConfig = dbConfigSection.Get<MbtDatabaseSettings>();
            x.UseMongoDB(dbConfig.ConnectionString, dbConfig.DatabaseName);
        });

        services.AddWorkflowSteps();
    }

    public static void UseWorkflowCore(this IServiceProvider serviceProvider)
    {
        var host = serviceProvider.GetService<IWorkflowHost>();
        RegisterClassMaps();
        RegisterWorkflows(host);
    }

    static void RegisterClassMaps()
    {
        // used to skip json->bson conversion in wf-core

        RegisterClassMap<WFData>();
        RegisterClassMap<SubmitTransferWorkflowDataV2>();
        RegisterClassMap<IncrementalFundsApproveWorkflowDataV2>();
        RegisterClassMap<IncrementalFundsApproveWorkflowData>();
        RegisterClassMap<CloneGroupedActivitiesWorkflowData>();
    }

    private static void RegisterClassMap<T>()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
            BsonClassMap.RegisterClassMap<T>(cm => { cm.AutoMap(); });
    }

    private static void RegisterWorkflows(IWorkflowHost host)
    {
        if (host != null)
        {
            host.RegisterWorkflow<SubmitTransferWorkflow, WFData>();
            host.RegisterWorkflow<SubmitTransferWorkflowV2, SubmitTransferWorkflowDataV2>();
            host.RegisterWorkflow<IncrementalFundsApproveWorkflow, IncrementalFundsApproveWorkflowData>();
            host.RegisterWorkflow<IncrementalFundsApproveWorkflowV2, IncrementalFundsApproveWorkflowDataV2>();
            host.RegisterWorkflow<CloneGroupedActivitiesWorkflow, CloneGroupedActivitiesWorkflowData>();
            host.Start();
        }
        else
        {
            throw new Exception("Can't find WorkflowHost service");
        }
    }

    private static void AddWorkflowSteps(this IServiceCollection services)
    {
        services.AddTransient<CheckGeoStep>();
        services.AddTransient<AddTaskStep>();
        services.AddTransient<ApproveTransferStep>();
        services.AddTransient<SendEmailStep>();
        services.AddTransient<CancelTaskStep>();
        services.AddTransient<AddChatMessageStep>();
        services.AddTransient<ResolveUserStep>();
        services.AddTransient<TaskResultStep>();
        services.AddTransient<WF.IncrementalFunds.Steps.TaskResultStep>();
        services.AddTransient<WF.IncrementalFunds.Steps.ApproveIncrementalFundStep>();
        services.AddTransient<WF.IncrementalFunds.Steps.RejectIncrementalFundStep>();
        services.AddTransient<WF.IncrementalFunds.Steps.SendBackIncrementalFundStep>();
        services.AddTransient<SetTransferStatusStep>();
        services.AddTransient<CancelTransferSubmitStep>();
        services.AddTransient<CancelActiveTasksStep>();
        services.AddTransient<CloneGroupedActivitiesStep>();
    }
}
