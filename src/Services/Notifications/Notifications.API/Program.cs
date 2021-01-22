//  <copyright file="Program.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Notifications.API
{
    using System;
    using System.IO;

    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

    using Serilog;

    public class Program
    {
        private static readonly string Namespace = typeof(Program).Namespace;

        private static readonly string AppName =
            Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static int Main(string[] args)
        {
            IConfiguration configuration = GetConfiguration();

            Log.Logger = CreateSerilogLogger(configuration);


            try
            {
                Log.Information("Configuring web host ({ApplicationContext})...", AppName);
                IWebHost host = CreateHostBuilder(args, configuration);

                Log.Information("Starting web host ({ApplicationContext})...", AppName);
                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IWebHost CreateHostBuilder(string[] args, IConfiguration configuration)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .CaptureStartupErrors(false)
                          .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
                          .UseStartup<Startup>()
                          .UseContentRoot(Directory.GetCurrentDirectory())
                          .UseSerilog()
                          .Build();
        }

        private static IConfiguration GetConfiguration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                                            .SetBasePath(Directory.GetCurrentDirectory())
                                            .AddJsonFile("appsettings.json", false, true)
                                            .AddEnvironmentVariables();

            return builder.Build();
        }

        private static ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                   .MinimumLevel.Verbose()
                   .Enrich.WithProperty("Notification Service", AppName)
                   .Enrich.FromLogContext()
                   .WriteTo.Console()
                   .ReadFrom.Configuration(configuration)
                   .CreateLogger();
        }
    }
}