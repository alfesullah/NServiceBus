namespace NServiceBus.Routing
{
    using System;

    /// <summary>
    /// Represents a route that should deliver the message to all interested subscribers
    /// </summary>
    public class ToAllSubscribers:RoutingStrategy
    {
        /// <summary>
        /// Initializes the strategy
        /// </summary>
        /// <param name="eventType">The event being published</param>
        public ToAllSubscribers(Type eventType)
        {
            EventType = eventType;
        }

        /// <summary>
        /// The event being published
        /// </summary>
        public Type EventType { get; private set; }
    }
}