using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MS_Application.Repositories.Interfaces;
using MS_Infrastructure.DataAccess;
using MS_Infrastructure.DataAccess.DISTS.Contexts;
using MS_Infrastructure.Repositories;
using System.Reflection;

namespace MS_Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection SetDBContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.RegisterServiceInterfaces();
        services.AddScoped<IUnitOfWork, UnitOfWork<CrmDbContext>>();
        services.AddScoped<IUnitOfWork, UnitOfWork<DistDbContext>>();

        services.AddScoped<ICrmUnitOfWork, CrmUnitOfWork>();
        services.AddScoped<IDistUnitOfWork, DistUnitOfWork>();

        return services;
    }

    private static void AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CrmDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("CRMConnection")));
        services.AddDbContext<DistDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DISTConnection")));
    }

    private static void RegisterServiceInterfaces(
        this IServiceCollection services)
    {
        var assembly = Assembly.Load("MS_Application");
        var types = assembly.GetTypes();

        foreach (var implementationType in types
            .Where(t => t.Name.EndsWith("Service")
                     && !t.IsAbstract
                     && !t.IsInterface))
        {
            var interfaces = implementationType.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                services.AddScoped(@interface, implementationType);
            }
        }
    }
}