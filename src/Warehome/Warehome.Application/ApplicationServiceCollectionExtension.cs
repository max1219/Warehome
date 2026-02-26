using Microsoft.Extensions.DependencyInjection;
using Warehome.Application.Services;
using Warehome.Application.Services.Implementations;

namespace Warehome.Application;

public static class ApplicationServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IStorageService, StorageService>();
        services.AddScoped<IStorageCategoryService, StorageCategoryService>();
        services.AddScoped<IItemTypeService, ItemTypeService>();
        services.AddScoped<IItemTypeCategoryService, ItemTypeCategoryService>();
        return services;
    }
}