using GeoTaskManager.Application.Core.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace GeoTaskManager.Api
{
    class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization",
            "CA1303:Do not pass literals as localized parameters",
            Justification = "<Pending>")]
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Initialize database connection and seed data
            var dbContext = host.Services
                .GetService<IGeoTaskManagerDbContext>();
            await dbContext
                .InitAsync()
                .ConfigureAwait(false);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
