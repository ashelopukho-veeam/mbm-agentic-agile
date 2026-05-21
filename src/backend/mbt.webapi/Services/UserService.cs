using System.Linq;
using System.Threading.Tasks;
using mbt.webapi.BuiltIn;
using mbt.webapi.Repositories;
using mbt.webapi.Services.Interfaces;
using UserProfile = mbt.webapi.Domain.Entities.UserProfile;

namespace mbt.webapi.Services;

public class UserService : IUserService
{
    private readonly IDbBaseRepository<UserProfile> _usersRepository;

    public UserService(IDbBaseRepository<UserProfile> usersRepository)
    {
        _usersRepository = usersRepository;
        EnsureSystemUser();
    }

    private void EnsureSystemUser()
    {
        var sysUser = _usersRepository.FindOne(u => u.Id == BuiltInConstants.SystemUserId);
        if (sysUser == null)
            _usersRepository.Create(
                new UserProfile()
                {
                    DisplayName = BuiltInConstants.SystemUserName,
                    Id = BuiltInConstants.SystemUserId,
                    Email = "",
                    IsGlobalApprover = false
                });
    }

    public async Task<PagedResult<UserProfile>> GetPaged(int page = 1, int pageSize = 5)
    {
        var users = await _usersRepository.GetAsync();

        var skip = (page - 1) * pageSize;
        var totalPages = users.Count / pageSize + 1;
        var usersPage = users.Skip(skip).Take(pageSize).ToList();

        return new PagedResult<UserProfile>()
        {
            Items = usersPage,
            TotalPages = totalPages,
        };
    }

    public UserProfile Get(string id)
    {
        return _usersRepository.Get(id);
    }

    public Task<UserProfile> GetAsync(string id)
    {
        return _usersRepository.GetAsync(id);
    }

    public Task<UserProfile> EnsureUser(UserProfile profile)
    {
        var checkExisted = _usersRepository.FindOne(u => u.Id == profile.Id);
        return checkExisted == null ? _usersRepository.CreateAsync(profile) : _usersRepository.UpdateAsync(profile);
    }
}
