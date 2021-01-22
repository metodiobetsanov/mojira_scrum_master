//  <copyright file="ConsulSettings.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace ServiceDiscovery.Settings
{
    /// <summary>
    ///     Consul Settings
    /// </summary>
    public class ConsulSettings
    {
        /// <summary>
        ///     Gets or sets the URL.
        /// </summary>
        /// <value>
        ///     The URL.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        ///     Gets or sets the name of the service.
        /// </summary>
        /// <value>
        ///     The name of the service.
        /// </value>
        public string ServiceName { get; set; }

        /// <summary>
        ///     Gets or sets the service address.
        /// </summary>
        /// <value>
        ///     The service address.
        /// </value>
        public string ServiceAddress { get; set; }

        /// <summary>
        ///     Gets or sets the port.
        /// </summary>
        /// <value>
        ///     The port.
        /// </value>
        public int Port { get; set; }
    }
}