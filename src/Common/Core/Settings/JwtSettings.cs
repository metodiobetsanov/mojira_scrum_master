//  <copyright file="JwtSettings.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Core.Settings
{
    /// <summary>
    ///     JWT Settings Holder
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        ///     Gets or sets the JWT public key.
        /// </summary>
        /// <value>
        ///     The JWT public key.
        /// </value>
        public string JwtPublicKey { get; set; }

        /// <summary>
        ///     Gets or sets the JWT private key.
        /// </summary>
        /// <value>
        ///     The JWT private key.
        /// </value>
        public string JwtPrivateKey { get; set; }

        /// <summary>
        ///     Gets or sets the issuer.
        /// </summary>
        /// <value>
        ///     The issuer.
        /// </value>
        public string Issuer { get; set; }

        /// <summary>
        ///     Gets or sets the audience.
        /// </summary>
        /// <value>
        ///     The audience.
        /// </value>
        public string Audience { get; set; }
    }
}