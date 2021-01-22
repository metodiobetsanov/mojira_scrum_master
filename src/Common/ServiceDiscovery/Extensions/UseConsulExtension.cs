//  <copyright file="UseConsulExtension.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace ServiceDiscovery.Extensions
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    using Consul;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using Polly;

    using Settings;

    using Policy = Polly.Policy;

    /// <summary>
    ///     Use Consul Extension
    /// </summary>
    public static class UseConsulExtension
    {
        /// <summary>
        ///     Uses Consul.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="tags">The tags.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, params string[] tags)
        {
            IHostApplicationLifetime lifetime = app.ApplicationServices
                                                   .GetRequiredService<IHostApplicationLifetime>();

            IConsulClient consulClient = app.ApplicationServices
                                            .GetRequiredService<IConsulClient>();

            ILoggerFactory loggingFactory = app.ApplicationServices
                                               .GetRequiredService<ILoggerFactory>();
            ILogger<IApplicationBuilder> logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            ConsulSettings consulSettings = app.ApplicationServices
                                               .GetRequiredService<ConsulSettings>();

            string name = Dns.GetHostName();
            IPAddress ip = Dns.GetHostEntry(name).AddressList
                              .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

            string port = consulSettings.Port > 0 ? $"{consulSettings.Port}" : "80";

            AgentServiceRegistration registration = new AgentServiceRegistration
                                                    {
                                                        ID = $"{consulSettings.ServiceName}@{ip}",
                                                        Name = consulSettings.ServiceName,
                                                        Address = $"http://{ip}",
                                                        Port = consulSettings.Port,
                                                        Tags = tags,
                                                        Check = new AgentCheckRegistration
                                                                {
                                                                    HTTP =
                                                                        $"http://{ip}:{port}/hc",
                                                                    Notes = "Checks hc on service",
                                                                    Interval = TimeSpan.FromSeconds(3),
                                                                    Timeout = TimeSpan.FromSeconds(3),
                                                                    DeregisterCriticalServiceAfter =
                                                                        TimeSpan.FromSeconds(21)
                                                                }
                                                    };

            lifetime.ApplicationStarted.Register(() =>
            {
                logger.LogInformation("Registering with Consul");
                Policy.Handle<Exception>().RetryForever().Execute(() =>
                {
                    consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                    consulClient.Agent.ServiceRegister(registration).Wait();
                });
            });


            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Deregistering from Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });

            return app;
        }
    }
}