using GeoTaskManager.Application.Core.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GeoTaskManager.MongoDb
{
    public static class ConfigureServiceExtensions
    {
        public static void AddGeoTaskMongoDb
            (this IServiceCollection services)
        {
            services.AddSingleton<IGeoTaskManagerDbContext, MongoDbContext>();
            services.AddMediatR(new Assembly[]
                {
                    Assembly.GetExecutingAssembly()
                });
        }
    }
}