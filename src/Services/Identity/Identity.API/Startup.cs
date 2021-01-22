//  <copyright file="Startup.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Identity.API
{
    using System;
    using System.Linq;

    using AutoMapper;

    using Core.Extensions;

    using Data;
    using Data.Entities;

    using HealthChecks.UI.Client;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Logging;

    using Service.Services;

    using Services;
    using Services.AutoMapper;
    using Services.Contracts;

    /// <summary>
    ///     Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        ///     The application name
        /// </summary>
        private readonly string AppName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.AppName = typeof(Startup).Namespace?.Split(new[] { '.' }).First();
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
            // Base Setup
            services.AddCustomApp();
            services.AddCustomSwagger(this.AppName);
            services.AddCustomAuthentication(this.Configuration);
            services.AddEventBus(this.Configuration);
            services.AddConsul(this.Configuration);

            // Specific Setup
            services.AddDbContext<IdentityAPIContext>(options =>
                                                          options.UseLazyLoadingProxies()
                                                                 .UseSqlServer(
                                                                     this.Configuration["ConnectionString"],
                                                                     sqlOptions =>
                                                                     {
                                                                         sqlOptions.MigrationsAssembly("Identity.Data");
                                                                         sqlOptions.EnableRetryOnFailure(
                                                                             15, TimeSpan.FromSeconds(30), null);
                                                                     }));

            services.AddIdentity<User, Role>(options =>
                    {
                        options.User.RequireUniqueEmail = true;
                        options.Password.RequiredUniqueChars = 1;
                        options.Password.RequiredLength = 8;
                    })
                    .AddDefaultTokenProviders()
                    .AddEntityFrameworkStores<IdentityAPIContext>();

            // Services
            services.AddAutoMapper(configuration => AutoMapping.Config());
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserService, UserService>();

            // Health Checks
            services.AddHealthChecks()
                    .AddCheck("self", () => HealthCheckResult.Healthy(), new[] { "identity-api" })
                    .AddSqlServer(this.Configuration["ConnectionString"],
                                  name: "IdentityDB-check",
                                  healthQuery: "SELECT 1;",
                                  tags: new[] { "IdentityDB" });
        }

        /// <summary>
        ///     Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseCustomSwagger();
            app.UseCustomApp();
            app.UseGlobalErrorHandler(loggerFactory.CreateLogger(this.AppName));
            app.UseCustomAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions
                                                 {
                                                     Predicate = _ => true,
                                                     ResponseWriter = UIResponseWriter
                                                         .WriteHealthCheckUIResponse
                                                 });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                                                       {
                                                           Predicate = r => r.Name.Contains("self")
                                                       });
            });

            app.UseConsul(this.AppName);
        }
    }
}