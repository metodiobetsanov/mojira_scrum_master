//  <copyright file="ListenForUserRegisteredEventExtension.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Notifications.API.Auth
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using Services.Events.Auth;

    public static class ListenForUserRegisteredEventExtension
    {
        private static UserRegisteredEventConsumer Listener { get; set; }

        public static IApplicationBuilder ListenForUserRegisteredEvent(this IApplicationBuilder app)
        {
            Listener = app.ApplicationServices.GetService<UserRegisteredEventConsumer>();
            IHostApplicationLifetime life = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            life.ApplicationStarted.Register(OnStarted);
            life.ApplicationStopping.Register(OnStopping);

            return app;
        }

        private static void OnStarted()
        {
            Listener.Consume();
        }

        private static void OnStopping()
        {
            Listener.Disconnect();
        }
    }
}