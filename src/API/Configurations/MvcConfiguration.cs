using FluentValidation.AspNetCore;
using System.Reflection;

namespace API.Configurations
{
    public static class MvcConfiguration
    {
        public static void AddMvcConfiguration(this IServiceCollection services)
        {
            services
                .AddControllers()
                .AddFluentValidation(options => options
                .RegisterValidatorsFromAssembly(Assembly.Load("AppServices")));
        }
    }
}
