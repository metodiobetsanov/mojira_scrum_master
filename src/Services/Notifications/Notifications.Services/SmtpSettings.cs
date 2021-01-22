//  <copyright file="SmtpSettings.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Notifications.Services
{
    public class SmtpSettings
    {
        public string Server { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool UseSsl { get; set; }
    }
}