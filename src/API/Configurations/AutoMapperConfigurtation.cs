using System.Reflection;

namespace API.Configurations;

public static class AutoMapperConfiguration
{
    public static void AddAutoMapperConfiguration(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var assembly = Assembly.Load("AppServices");

        services
            .AddAutoMapper((serviceProvider, mapperConfiguration) => mapperConfiguration.AddMaps(assembly), assembly);
    }
}