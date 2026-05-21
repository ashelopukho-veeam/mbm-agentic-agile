using System.Collections.Generic;
using System.Linq;
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

public class GetNotifications : EndpointBaseAsync.WithoutRequest.WithResult<List<NotificationEvent>>
{
    private readonly INotificationService _notificationService;

    public GetNotifications(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [Authorize(Roles = AppRoles.AdminPolicy)]
    [HttpGet("api/notifications")]
    [SwaggerOperation(
        Summary = "Get notifications",
        Description = "Get notifications",
        OperationId = "Notifications.Get",
        Tags = new[] { "Notifications" })]
    public override async Task<List<NotificationEvent>> HandleAsync(CancellationToken cancellationToken = new())
    {
        var result = await _notificationService.GetNotificationEvents();
        return result.ToList();
    }
}

public class AlertDto : BaseItemDto
{
}
