using mbt.webapi.Domain;

namespace mbt.webapi.Services;

public static class PaidMediaHelper
{
    public static void SetPaidMediaFields(IWithPaidMediaData destination, IWithPaidMediaData source)
    {
        destination.GlobalRegion = source?.GlobalRegion;
        destination.SubRegion = source?.SubRegion;
        destination.GlobalProgram = source?.GlobalProgram;
        destination.Team = source?.Team;
        destination.AdService = source?.AdService;
        destination.Management = source?.Management;
        destination.ExecutionTeam = source?.ExecutionTeam;
        destination.ContentType = source?.ContentType;
    }
}
