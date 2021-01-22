//  <copyright file="UserRegisterEvent.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Core.Events.Auth
{
    /// <summary>
    ///     User Register Event
    /// </summary>
    /// <seealso cref="BaseEvent" />
    public class UserRegisterEvent : BaseEvent
    {
        /// <summary>
        ///     Gets or sets the name of the user.
        /// </summary>
        /// <value>
        ///     The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        ///     Gets or sets the email.
        /// </summary>
        /// <value>
        ///     The email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        ///     Gets or sets the full name.
        /// </summary>
        /// <value>
        ///     The full name.
        /// </value>
        public string FullName { get; set; }
    }
}