using JetBrains.Annotations;

namespace mbt.webapi.Endpoints.GroupedActivities;

[PublicAPI]
public class GetByIdGroupedActivityRequest
{
    public string GroupedActivityId { get; set; }
}
