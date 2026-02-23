using System.Text.Json.Serialization;

namespace Warehome.Web;

public static class WebServiceCollectionExtension
{
    public static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }
}