using System.Collections.Generic;
using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Services.Interfaces;
using MongoDB.Driver;

namespace mbt.webapi.Repositories;

public interface IMailRepository : IDbBaseRepository<MailEntity>
{
    Task<List<MailEntity>> GetUnprocessed();
}

public class MailRepository : DbBaseRepository<MailEntity>, IMailRepository
{
    public MailRepository(IDbContext context, ICurrentUserContext currentUserContext) : base(context,
        currentUserContext)
    {
    }

    public Task<List<MailEntity>> GetUnprocessed()
    {
        return BuildAggregate<MailEntity>().Match(entity => entity.Status == NotificationStatus.Unprocessed)
            .ToListAsync();
    }
}
