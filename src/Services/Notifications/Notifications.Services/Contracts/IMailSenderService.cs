//  <copyright file="IMailSenderService.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Notifications.Services.Contracts
{
    using System.Threading.Tasks;

    using MimeKit;

    public interface IMailSenderService
    {
        Task SendNotificationMail(BodyBuilder messageBody, string subject, MailboxAddress toAddress);
    }
}