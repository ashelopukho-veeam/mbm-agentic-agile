using System;

namespace mbt.webapi.Endpoints.Notifications;

public class CreateEventRequest
{
    public DateTimeOffset EventDate { get; set; }
}
