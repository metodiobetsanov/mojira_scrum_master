//  <copyright file="Startup.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Gateway.API
{
    using System;

    using HealthChecks.UI.Client;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;

    using Ocelot.DependencyInjection;
    using Ocelot.Middleware;
    using Ocelot.Provider.Consul;

    /// <summary>
    ///     Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        ///     Gets the configuration.
        /// </summary>
        /// <value>
        ///     The configuration.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        ///     Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddOcelot()
                    .AddConsul();

            // Health Checks
            services.AddHealthChecks()
                    .AddCheck("self", () => HealthCheckResult.Healthy())
                    .AddConsul(setup =>
                               {
                                   setup.HostName = this.Configuration["ConsulConnection"];
                                   setup.Port = 8500;
                                   setup.RequireHttps = false;
                               },
                               "service-discovery-check",
                               tags: new[] { "service-discovery" })
                    .AddRabbitMQ(
                        $"amqp://admin:password@{this.Configuration["EventBusConnection"]}",
                        name: "event-bus-check",
                        tags: new[] { "event-bus" })
                    .AddUrlGroup(new Uri(this.Configuration["IDENTITY_SERVICE_HC"]), "identity-api-check",
                                 tags: new[] { "identity-api" })
                    .AddUrlGroup(new Uri(this.Configuration["NOTIFICATIONS_SERVICE_HC"]), "notifications-api-check",
                                 tags: new[] { "notifications-api" });
        }

        /// <summary>
        ///     Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions
                                                 {
                                                     Predicate = _ => true,
                                                     ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                                                 });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                                                       {
                                                           Predicate = r => r.Name.Contains("self")
                                                       });
            });

            app.UseOcelot().GetAwaiter();
        }
    }
}