using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace mbt.webapi.Configuration.Validation;

public class AccessDeniedProblemDetails : ProblemDetails
{
    public AccessDeniedProblemDetails(AccessDeniedException exception)
    {
        Title = "Access denied";
        Status = StatusCodes.Status403Forbidden;
        Type = "access-denied";
        Detail = "Access denied";
    }
}
