using System.Threading.Tasks;
using UserProfile = mbt.webapi.Domain.Entities.UserProfile;


namespace mbt.webapi.Services.Interfaces;

public interface IUserService : IBaseService
{
    Task<PagedResult<UserProfile>> GetPaged(int page = 1, int pageSize = 5);
    UserProfile Get(string id);
    Task<UserProfile> GetAsync(string id);
    Task<UserProfile> EnsureUser(UserProfile profile);
}
