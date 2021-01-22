//  <copyright file="AddEventBusExtension.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace EventBus.Extensions
{
    using Contracts;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using RabbitMQ.Client;

    using Settings;

    /// <summary>
    ///     Add EventBus Extension
    /// </summary>
    public static class AddEventBusExtension
    {
        /// <summary>
        ///     Adds the event bus.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            EventBusSettings eventBusSettings = configuration.GetSection("EventBusSettings").Get<EventBusSettings>();

            services.AddSingleton(eventBusSettings);
            services.AddSingleton<IEventBusPersistentConnection>(sp =>
            {
                ILogger<EventBusPersistentConnection> logger =
                    sp.GetRequiredService<ILogger<EventBusPersistentConnection>>();

                ConnectionFactory factory = new ConnectionFactory
                                            {
                                                HostName = eventBusSettings.Connection,
                                                UserName = eventBusSettings.Username,
                                                Password = eventBusSettings.Password
                                            };

                return new EventBusPersistentConnection(factory, logger, eventBusSettings.RetryCount);
            });
            services.AddScoped(typeof(IEventProducer<>), typeof(EventProducer<>));

            return services;
        }
    }
}