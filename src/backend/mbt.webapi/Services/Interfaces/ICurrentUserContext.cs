using System.Collections.Generic;

namespace mbt.webapi.Services.Interfaces;

public interface ICurrentUserContext
{
    bool UseSystemAccount { get; set; }
    string UserId { get; }
    string UserName { get; }
    string Email { get; }

    bool IsInRole(string role);

    bool IsInRoles(IEnumerable<string> roles);

    bool IsInRoles(params string[] roles);
}
