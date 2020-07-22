using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GeoTaskManager.MongoDb
{
    public static class ConfigureServiceExtensions
    {
        public static void AddGeoTaskApplication
            (this IServiceCollection services)
        {
            services.AddMediatR(new Assembly[]
                {
                    Assembly.GetExecutingAssembly()
                });
        }
    }
}