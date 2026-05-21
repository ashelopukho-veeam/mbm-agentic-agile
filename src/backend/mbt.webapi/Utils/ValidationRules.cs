using System;
using System.Threading;
using System.Threading.Tasks;
using mbt.webapi.Services.Interfaces;

namespace mbt.webapi.Utils;

public static class ValidationRules
{
    public static async Task<bool> IsUserExists(IUserService userService, Guid? userId, CancellationToken token)
    {
        if (userId == null)
            return false;

        var user = await userService.GetAsync(userId.ToString());
        return user != null;
    }
}
