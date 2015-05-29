namespace NServiceBus.Routing
{
    using System;

    /// <summary>
    /// Provides routing for messages being sent
    /// </summary>
    abstract class MessageRouter
    {
        /// <summary>
        /// Tries to find a route for the given message type
        /// </summary>
        /// <param name="messageType">The message type being sent</param>
        /// <param name="destination">The destination for the message if found</param>
        /// <returns>True of route is found</returns>
        public abstract bool TryGetRoute(Type messageType, out string destination);
    }
}