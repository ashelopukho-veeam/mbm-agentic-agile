using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace mbt.webapi.Endpoints.Admin.WF;

public class ListWorkflowsInstances : EndpointBaseAsync.WithoutRequest.WithResult<List<WorkflowEntity>>
{
    private readonly IDbBaseRepository<WorkflowEntity> _workflowRepository;


    public ListWorkflowsInstances(IDbBaseRepository<WorkflowEntity> workflowRepository)
    {
        _workflowRepository = workflowRepository;
    }

    [HttpGet("api/admin/workflows")]
    public override Task<List<WorkflowEntity>> HandleAsync(
        CancellationToken cancellationToken = new CancellationToken())
    {
        return _workflowRepository.GetAsync();
    }
}
