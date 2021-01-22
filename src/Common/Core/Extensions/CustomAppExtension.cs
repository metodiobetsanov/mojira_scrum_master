//  <copyright file="CustomAppExtension.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Core.Extensions
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Microsoft.Extensions.DependencyInjection;

    public static class CustomAppExtension
    {
        private const string CorsPolicy = "CorsPolicy";

        /// <summary>
        ///     Adds the custom application.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddCustomApp(this IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });

            services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });

            services.AddControllers()
                    .AddNewtonsoftJson();
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicy,
                                  builder => builder
                                             .SetIsOriginAllowed(host => true)
                                             .AllowAnyMethod()
                                             .AllowAnyHeader()
                                             .AllowCredentials());
            });

            return services;
        }

        public static IApplicationBuilder UseCustomApp(this IApplicationBuilder app)
        {
            app.UseCors(CorsPolicy);
            app.UseRouting();

            return app;
        }
    }
}