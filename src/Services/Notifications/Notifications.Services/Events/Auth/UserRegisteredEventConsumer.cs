//  <copyright file="UserRegisteredEventConsumer.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace Notifications.Services.Events.Auth
{
    using System.Text;

    using Contracts;

    using Core.Constants;
    using Core.Contracts;
    using Core.Events.Auth;

    using global::AutoMapper;

    using MimeKit;

    using Newtonsoft.Json;

    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    public class UserRegisteredEventConsumer
    {
        private readonly IEventBusPersistentConnection connection;
        private readonly IMailSenderService mailSenderService;
        private readonly IMapper mapper;
        private readonly string messageSubject = "New User Registered";

        public UserRegisteredEventConsumer(IEventBusPersistentConnection connection, IMapper mapper,
                                           IMailSenderService mailSenderService)
        {
            this.connection = connection;
            this.mapper = mapper;
            this.mailSenderService = mailSenderService;
        }

        public void Consume()
        {
            IModel channel = this.connection.CreateModel();
            channel.QueueDeclare(EventBusConstants.MailSendQueue, false, false,
                                 false, null);

            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            //Create event when something receive
            consumer.Received += this.ReceivedEvent;

            channel.BasicConsume(EventBusConstants.MailSendQueue, true, consumer);
        }

        private void ReceivedEvent(object sender, BasicDeliverEventArgs e)
        {
            string eventMessage = Encoding.UTF8.GetString(e.Body.Span);
            UserRegisterEvent model = JsonConvert.DeserializeObject<UserRegisterEvent>(eventMessage);


            MailboxAddress to = new MailboxAddress(model.UserName, model.Email);

            BodyBuilder bodyBuilder = new BodyBuilder();

            this.mailSenderService.SendNotificationMail(bodyBuilder, this.messageSubject, to);
        }

        public void Disconnect()
        {
            this.connection.Dispose();
        }
    }
}