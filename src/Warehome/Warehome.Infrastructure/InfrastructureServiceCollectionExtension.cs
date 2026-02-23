using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Warehome.Application.Repositories;
using Warehome.Domain.Entities;
using Warehome.Infrastructure.Data;
using Warehome.Infrastructure.Data.Repositories;

namespace Warehome.Infrastructure;

public static class InfrastructureServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => 
            options.UseSqlite(configuration.GetConnectionString("Default")));
        services.AddScoped<IStorageRepository, EfStorageRepository>();
        services.AddScoped<ICategoryRepository<Storage>, EfStorageCategoryRepository>();
        return services;
    }
}