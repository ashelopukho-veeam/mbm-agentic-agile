using JetBrains.Annotations;

namespace mbt.webapi.Endpoints.GroupedActivities;

[PublicAPI]
public class DeleteGroupedActivityRequest
{
    public string GroupedActivityId { get; set; }
}
