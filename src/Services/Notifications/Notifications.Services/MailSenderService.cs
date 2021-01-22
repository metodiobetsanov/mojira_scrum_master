//  <copyright file="MailSenderService.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Notifications.Services
{
    using System.Threading.Tasks;

    using Contracts;

    using MailKit.Net.Smtp;

    using MimeKit;

    public class MailSenderService : IMailSenderService
    {
        private readonly SmtpSettings settings;

        private SmtpClient client;

        private bool isInitialized;

        public MailSenderService(SmtpSettings settings)
        {
            this.settings = settings;
            if (!this.isInitialized)
            {
                this.client = new SmtpClient();
                this.isInitialized = true;
            }
        }

        public async Task SendNotificationMail(BodyBuilder messageBody, string subject, MailboxAddress toAddress)
        {
            if (!this.isInitialized)
            {
                this.client = new SmtpClient();
                this.isInitialized = true;
            }

            if (!this.client.IsConnected)
                await this.client.ConnectAsync(this.settings.Server, this.settings.Port, this.settings.UseSsl);

            if (!this.client.IsAuthenticated)
                await this.client.AuthenticateAsync(this.settings.Username, this.settings.Password);

            MimeMessage message = new MimeMessage();

            MailboxAddress from = new MailboxAddress("Mojira Notifications",
                                                     "notifications@example.com");
            message.From.Add(from);
            message.To.Add(toAddress);

            message.Subject = "New User Registration";
            message.Body = messageBody.ToMessageBody();

            await this.client.SendAsync(message);
            await this.client.DisconnectAsync(true);

            this.client.Dispose();
            this.isInitialized = false;
        }
    }
}