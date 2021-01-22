//  <copyright file="AddConsulExtension.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace ServiceDiscovery.Extensions
{
    using System;

    using Consul;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Settings;

    /// <summary>
    ///     Add Consul Extension
    /// </summary>
    public static class AddConsulExtension
    {
        /// <summary>
        ///     Adds Consul.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            ConsulSettings consulSettings = configuration.GetSection("ConsulSettings").Get<ConsulSettings>();

            services.AddSingleton(consulSettings);
            services.AddSingleton<IConsulClient>(sp =>
            {
                return new ConsulClient(cfg =>
                {
                    if (!string.IsNullOrEmpty(consulSettings.Url))
                        cfg.Address = new Uri(consulSettings.Url);
                });
            });

            return services;
        }
    }
}