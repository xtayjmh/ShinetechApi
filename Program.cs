using API.Models;
using API.Models.Data;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            // CreateHostBuilder(args).Build().Run();
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<GeneralDbContext>();
                    DbInitializer.Initialize(context);
                }
                catch (Exception e)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(e, "An error occurred while seeding  the database.");
                }
            }


            host.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.AddFilter("System", LogLevel.Warning);
                        logging.AddFilter("Microsoft", LogLevel.Warning);
                        logging.AddDebug();
                        //logging.AddConsole();
                        logging.AddLog4Net();
                    })
                    .UseStartup<Startup>().UseKestrel(options =>
                    {
                        options.Limits.MaxRequestBodySize = null;
                    }).UseUrls("http://*:9998");
                })
            .UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}

