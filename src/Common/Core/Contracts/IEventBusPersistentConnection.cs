//  <copyright file="IEventBusPersistentConnection.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Core.Contracts
{
    using System;

    using RabbitMQ.Client;

    /// <summary>
    ///     EventBus Persistent Connection Interface
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IEventBusPersistentConnection : IDisposable
    {
        /// <summary>
        ///     Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        bool IsConnected { get; }

        /// <summary>
        ///     Tries to connect.
        /// </summary>
        /// <returns></returns>
        bool TryConnect();

        /// <summary>
        ///     Creates the model.
        /// </summary>
        /// <returns></returns>
        IModel CreateModel();
    }
}