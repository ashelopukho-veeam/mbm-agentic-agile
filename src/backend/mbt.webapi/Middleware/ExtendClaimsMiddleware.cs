using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace mbt.webapi.Middleware;

public class ExtendClaimsMiddleware
{
    private readonly RequestDelegate _next;

    public ExtendClaimsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var rolesHeader = context.Request.Headers["X-Test-Roles"];
        var testRoles = rolesHeader.ToString().Split(",");

        if (string.IsNullOrEmpty(rolesHeader)
            || testRoles.Length == 0)
        {
            await _next(context);
            return;
        }


        if (context.User.Identity is ClaimsIdentity identity)
        {
            // remove original roles
            var rolesClaims = identity.FindAll(ClaimTypes.Role).ToList();
            foreach (var t in rolesClaims)
            {
                identity.RemoveClaim(t);
            }

            // set new roles
            foreach (var role in testRoles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
        }

        // Call the next middleware in the pipeline
        await _next(context);
    }
}
