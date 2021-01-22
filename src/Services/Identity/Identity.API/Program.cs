//  <copyright file="Program.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.API
{
    using System;
    using System.IO;

    using Data;

    using Extensions;

    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Seed;

    using Serilog;

    using ILogger = Serilog.ILogger;

    /// <summary>
    ///     Program
    /// </summary>
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

                Log.Information("Applying migrations ({ApplicationContext})...", AppName);
                host.MigrateSQLContext<IdentityAPIContext>((context, services) =>
                {
                    ILogger<IdentityContextSeed> logger = services.GetService<ILogger<IdentityContextSeed>>();

                    new IdentityContextSeed()
                        .SeedAsync(context, logger)
                        .Wait();
                });

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

        /// <summary>
        ///     Creates the host.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Gets the configuration.
        /// </summary>
        /// <returns></returns>
        private static IConfiguration GetConfiguration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                                            .SetBasePath(Directory.GetCurrentDirectory())
                                            .AddJsonFile("appsettings.json", false, true)
                                            .AddEnvironmentVariables();

            return builder.Build();
        }

        /// <summary>
        ///     Creates the serilog logger.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        private static ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            return new LoggerConfiguration()
                   .MinimumLevel.Verbose()
                   .Enrich.WithProperty("Identity Service", AppName)
                   .Enrich.FromLogContext()
                   .WriteTo.Console()
                   .ReadFrom.Configuration(configuration)
                   .CreateLogger();
        }
    }
}