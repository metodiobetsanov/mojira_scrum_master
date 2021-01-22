//  <copyright file="Startup.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Notifications.API
{
    using System.Linq;

    using Auth;

    using AutoMapper;

    using Core.Extensions;

    using HealthChecks.UI.Client;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    using Services;
    using Services.AutoMapper;
    using Services.Contracts;
    using Services.Events.Auth;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Base Setup
            services.AddCustomApp();
            services.AddCustomSwagger(typeof(Startup).Namespace?.Split(new[] { '.' }).First());
            services.AddCustomAuthentication(this.Configuration);
            services.AddEventBus(this.Configuration);
            services.AddConsul(this.Configuration);

            // Specific Setup

            // Services
            services.AddAutoMapper(configuration => AutoMapping.Config());
            services.AddSingleton(this.Configuration.GetSection("SmtpSettings").Get<SmtpSettings>());
            services.AddSingleton<IMailSenderService, MailSenderService>();

            services.AddSingleton<UserRegisteredEventConsumer>();

            // Health Checks
            services.AddHealthChecks()
                    .AddCheck("self", () => HealthCheckResult.Healthy(), new[] { "notifications-api" });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCustomApp();
            app.UseCustomAuthentication();

            app.ListenForUserRegisteredEvent();

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

            app.UseConsul(typeof(Startup).Namespace?.Split(new[] { '.' }).First());
        }
    }
}