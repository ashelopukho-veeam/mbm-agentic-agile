using System;
using mbt.webapi.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace mbt.webapi.Configuration;

public static class ServicesConfiguration
{
    public static void AddServices(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromCallingAssembly()
            .AddClasses(classes =>
                classes.AssignableTo(typeof(IBaseService)))
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        Console.WriteLine("Services added");
    }
}
