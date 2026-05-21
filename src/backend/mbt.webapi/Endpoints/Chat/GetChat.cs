using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.Domain.Entities;
using mbt.webapi.UseCases.Chat;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Chat;

public class GetChat : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithActionResult<List<ChatMessageExpanded>>
{
    private readonly IMediator _mediator;

    public GetChat(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("api/chat/{Id}")]
    [SwaggerOperation(
        Summary = "Get a chat by Id",
        Description = "Get a chat by Id",
        OperationId = "Chat.GetById",
        Tags = new[] { "Chat" })]
    public override async Task<ActionResult<List<ChatMessageExpanded>>> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        var messages = await _mediator.Send(new GetChatByIdExpandedRequest { Id = request.Id }, cancellationToken);
        return messages;
    }
}
