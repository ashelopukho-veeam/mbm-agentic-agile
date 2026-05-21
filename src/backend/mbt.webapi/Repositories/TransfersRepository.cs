using mbt.webapi.Domain.Entities;
using mbt.webapi.Services.Interfaces;

namespace mbt.webapi.Repositories;

public interface ITransfersRepository : IDbBaseRepositoryExpandable<Transfer, TransferExpanded>
{
}

public class TransfersRepository : DbBaseRepositoryExpandable<Transfer, TransferExpanded>,
    ITransfersRepository
{
    public TransfersRepository(IDbContext context, ICurrentUserContext currentUserContext) : base(context,
        currentUserContext)
    {
        ExpandLookupsMapping = LookupMappings.TransfersExpandedMapping;
    }
}
