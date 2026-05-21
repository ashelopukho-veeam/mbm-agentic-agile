using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Notifications;

public class SetNotification : EndpointBaseAsync.WithRequest<CreateEventRequest>.WithResult<NotificationEvent>
{
    private readonly INotificationService _notificationService;

    public SetNotification(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [Authorize(Roles = AppRoles.AdminPolicy)]
    [HttpPost("api/notifications")]
    [SwaggerOperation(
        Summary = "Set a notification",
        Description = "Set a notification",
        OperationId = "Notifications.Set",
        Tags = new[] { "Notifications" })]
    public override async Task<NotificationEvent> HandleAsync(CreateEventRequest request,
        CancellationToken cancellationToken = new())
    {
        var notificationEvent = new NotificationEvent
        {
            EventDate = request.EventDate.UtcDateTime
        };


        await _notificationService.AddAsync(notificationEvent);

        return notificationEvent;
    }
}
