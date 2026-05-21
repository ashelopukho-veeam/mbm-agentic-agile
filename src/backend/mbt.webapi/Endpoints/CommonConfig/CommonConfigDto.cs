using JetBrains.Annotations;

namespace mbt.webapi.Endpoints.CommonConfig;

[PublicAPI]
public class CommonConfigDto
{
    public bool AllowCreateTransfers { get; set; }
    public bool AllowCreateReforecasts { get; set; }
    public bool NotificationBarMessageEnabled { get; set; }
    public string NotificationBarMessage { get; set; }
}
