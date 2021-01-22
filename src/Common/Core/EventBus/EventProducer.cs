//  <copyright file="EventProducer.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace EventBus
{
    using System;
    using System.Text;

    using Core.Contracts;
    using Core.Events;

    using Newtonsoft.Json;

    using RabbitMQ.Client;

    /// <summary>
    ///     Event Producer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IEventProducer{T}" />
    public class EventProducer<T> : IEventProducer<T> where T : BaseEvent
    {
        /// <summary>
        ///     The connection
        /// </summary>
        private readonly IEventBusPersistentConnection connection;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EventProducer{T}" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public EventProducer(IEventBusPersistentConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        ///     Publishes the event.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="publishModel">The publish model.</param>
        public void PublishEvent(string queue, T publishModel)
        {
            using (IModel channel = this.connection.CreateModel())
            {
                channel.QueueDeclare(queue, false, false, false, null);
                string message = JsonConvert.SerializeObject(publishModel);
                byte[] body = Encoding.UTF8.GetBytes(message);

                IBasicProperties properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.DeliveryMode = 2;

                channel.ConfirmSelect();
                channel.BasicPublish("", queue, true, properties, body);
                channel.WaitForConfirmsOrDie();

                channel.BasicAcks += (sender, eventArgs) => { Console.WriteLine("Sent RabbitMQ"); };
                channel.ConfirmSelect();
            }
        }
    }
}