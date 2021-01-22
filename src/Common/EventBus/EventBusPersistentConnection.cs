//  <copyright file="EventBusPersistentConnection.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace EventBus
{
    using System;
    using System.IO;
    using System.Net.Sockets;

    using Contracts;

    using Microsoft.Extensions.Logging;

    using Polly;
    using Polly.Retry;

    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using RabbitMQ.Client.Exceptions;

    /// <summary>
    ///     EventBus Persistent Connection
    /// </summary>
    /// <seealso cref="EventBus.Contracts.IEventBusPersistentConnection" />
    public class EventBusPersistentConnection : IEventBusPersistentConnection
    {
        /// <summary>
        ///     The connection factory
        /// </summary>
        private readonly IConnectionFactory connectionFactory;

        /// <summary>
        ///     The logger
        /// </summary>
        private readonly ILogger<EventBusPersistentConnection> logger;

        /// <summary>
        ///     The retry count
        /// </summary>
        private readonly int retryCount;

        /// <summary>
        ///     The synchronize root
        /// </summary>
        private readonly object syncRoot = new object();

        /// <summary>
        ///     The connection
        /// </summary>
        private IConnection connection;

        /// <summary>
        ///     The disposed
        /// </summary>
        private bool disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EventBusPersistentConnection" /> class.
        /// </summary>
        /// <param name="connectionFactory">The connection factory.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="retryCount">The retry count.</param>
        public EventBusPersistentConnection(IConnectionFactory connectionFactory,
                                            ILogger<EventBusPersistentConnection> logger, int retryCount = 5)
        {
            this.connectionFactory = connectionFactory;
            this.logger = logger;
            this.retryCount = retryCount;

            if (!this.IsConnected) this.TryConnect();
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected => this.connection != null && this.connection.IsOpen && !this.disposed;

        /// <summary>
        ///     Creates the model.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">No RabbitMQ connections are available to perform this action</exception>
        public IModel CreateModel()
        {
            if (!this.IsConnected)
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");

            return this.connection.CreateModel();
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.disposed) return;

            this.disposed = true;

            try
            {
                this.connection.Dispose();
            }
            catch (IOException ex)
            {
                this.logger.LogCritical(ex.ToString());
            }
        }

        /// <summary>
        ///     Tries to connect.
        /// </summary>
        /// <returns></returns>
        public bool TryConnect()
        {
            this.logger.LogInformation("RabbitMQ Client is trying to connect");

            lock (this.syncRoot)
            {
                RetryPolicy policy = Policy.Handle<SocketException>()
                                           .Or<BrokerUnreachableException>()
                                           .WaitAndRetry(this.retryCount,
                                                         retryAttempt =>
                                                             TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                                         (ex, time) =>
                                                         {
                                                             this.logger.LogWarning(
                                                                 ex,
                                                                 "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})",
                                                                 $"{time.TotalSeconds:n1}", ex.Message);
                                                         }
                                           );

                policy.Execute(() =>
                {
                    this.connection = this.connectionFactory
                                          .CreateConnection();
                });

                if (this.IsConnected)
                {
                    this.connection.ConnectionShutdown += this.OnConnectionShutdown;
                    this.connection.CallbackException += this.OnCallbackException;
                    this.connection.ConnectionBlocked += this.OnConnectionBlocked;

                    this.logger.LogInformation(
                        "RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events",
                        this.connection.Endpoint.HostName);

                    return true;
                }

                this.logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                return false;
            }
        }

        /// <summary>
        ///     Called when [connection blocked].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ConnectionBlockedEventArgs" /> instance containing the event data.</param>
        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (this.disposed) return;

            this.logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

            this.TryConnect();
        }

        /// <summary>
        ///     Called when [callback exception].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="CallbackExceptionEventArgs" /> instance containing the event data.</param>
        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (this.disposed) return;

            this.logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

            this.TryConnect();
        }

        /// <summary>
        ///     Called when [connection shutdown].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="reason">The <see cref="ShutdownEventArgs" /> instance containing the event data.</param>
        private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (this.disposed) return;

            this.logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

            this.TryConnect();
        }
    }
}