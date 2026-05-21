using mbt.webapi.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace mbt.webapi.Configuration;

public static class RepositoriesConfiguration
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IDbContext, DbContext>();
        services.Scan(scan => scan
            .FromCallingAssembly()
            .AddClasses(classes =>
                classes.AssignableTo(typeof(IDbBaseRepository<>)))
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
    }
}
