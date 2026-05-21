using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.Services.Interfaces;
using MediatR;

namespace mbt.webapi.UseCases.GA;

public class CloneGroupedActivitiesRequest : IRequest
{
    public List<Tuple<string, string>> FromTo { get; set; }
}

public class CloneGroupedActivitiesRequestHandler : IRequestHandler<CloneGroupedActivitiesRequest>
{
    private readonly IGroupActivitiesService _groupActivitiesService;

    public CloneGroupedActivitiesRequestHandler(IGroupActivitiesService groupActivitiesService)
    {
        _groupActivitiesService = groupActivitiesService;
    }

    public async Task Handle(CloneGroupedActivitiesRequest request, CancellationToken cancellationToken)
    {
        foreach (var (from, to) in request.FromTo) await _groupActivitiesService.Clone(from, to);
    }
}
