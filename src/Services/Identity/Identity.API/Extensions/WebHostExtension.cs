//  <copyright file="WebHostExtension.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.API.Extensions
{
    using System;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Polly;
    using Polly.Retry;

    /// <summary>
    ///     WebHostExtension
    /// </summary>
    public static class WebHostExtension
    {
        /// <summary>
        ///     Migrates the SQL context.
        /// </summary>
        /// <typeparam name="TContext">The type of the context.</typeparam>
        /// <param name="host">The host.</param>
        /// <param name="seeder">The seeder.</param>
        /// <returns></returns>
        public static IWebHost MigrateSQLContext<TContext>(this IWebHost host,
                                                           Action<TContext, IServiceProvider> seeder)
            where TContext : DbContext
        {
            using (IServiceScope scope = host.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;

                ILogger<TContext> logger = services.GetRequiredService<ILogger<TContext>>();

                TContext? context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation("Migrating database associated with context {DbContextName}",
                                          typeof(TContext).Name);

                    RetryPolicy retry = Policy.Handle<SqlException>()
                                              .WaitAndRetry(new[]
                                                            {
                                                                TimeSpan.FromSeconds(3),
                                                                TimeSpan.FromSeconds(5),
                                                                TimeSpan.FromSeconds(8)
                                                            });

                    retry.Execute(() => InvokeSeeder(seeder, context, services));

                    logger.LogInformation("Migrated database associated with context {DbContextName}",
                                          typeof(TContext).Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex, "An error occurred while migrating the database used on context {DbContextName}",
                        typeof(TContext).Name);
                }
            }

            return host;
        }

        /// <summary>
        ///     Invokes the seeder.
        /// </summary>
        /// <typeparam name="TContext">The type of the context.</typeparam>
        /// <param name="seeder">The seeder.</param>
        /// <param name="context">The context.</param>
        /// <param name="services">The services.</param>
        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context,
                                                   IServiceProvider services)
            where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}