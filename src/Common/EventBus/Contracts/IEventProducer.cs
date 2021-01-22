//  <copyright file="IEventProducer.cs">
//     Copyright (c) 2021  All rights reserved.
//     <author> MDO </author>
// </copyright>

namespace EventBus.Contracts
{
    using Events;

    /// <summary>
    ///     Event Producer Interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEventProducer<T> where T : BaseEvent
    {
        /// <summary>
        ///     Publishes the event.
        /// </summary>
        /// <param name="queue">The queue.</param>
        /// <param name="publishModel">The publish model.</param>
        void PublishEvent(string queue, T publishModel);
    }
}