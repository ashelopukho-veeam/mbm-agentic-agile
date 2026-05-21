using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace mbt.webapi.Configuration;

public static class MediatrConfiguration
{
    public static IServiceCollection AddMediatrConfiguration(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        return services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(assembly); });
    }
}
