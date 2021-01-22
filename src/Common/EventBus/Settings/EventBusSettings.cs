//  <copyright file="EventBusSettings.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace EventBus.Settings
{
    /// <summary>
    ///     EventBus Settings
    /// </summary>
    public class EventBusSettings
    {
        /// <summary>
        ///     Gets or sets the connection.
        /// </summary>
        /// <value>
        ///     The connection.
        /// </value>
        public string Connection { get; set; }

        /// <summary>
        ///     Gets or sets the username.
        /// </summary>
        /// <value>
        ///     The username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        /// <value>
        ///     The password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        ///     Gets or sets the retry count.
        /// </summary>
        /// <value>
        ///     The retry count.
        /// </value>
        public int RetryCount { get; set; }
    }
}