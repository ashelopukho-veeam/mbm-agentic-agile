using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Notifications;

public class DeleteNotification : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithoutResult
{
    private readonly INotificationService _notificationService;

    public DeleteNotification(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [Authorize(Roles = AppRoles.AdminPolicy)]
    [HttpDelete("api/notifications/{Id}")]
    [SwaggerOperation(
        Summary = "Remove notification",
        Description = "Remove a notification",
        OperationId = "Notifications.Delete",
        Tags = new[] { "Notifications" })]
    public override async Task HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        await _notificationService.RemoveAsync(request.Id);
    }
}
