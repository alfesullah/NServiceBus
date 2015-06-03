namespace NServiceBus.Routing
{
    /// <summary>
    /// Represents a route directly to the specified destination
    /// </summary>
    public class DirectToTargetDestination : RoutingStrategy
    {
        /// <summary>
        /// The destination
        /// </summary>
        public string Destination { get; private set; }

        /// <summary>
        /// Initializes the strategy
        /// </summary>
        /// <param name="destination">The destination</param>
        public DirectToTargetDestination(string destination)
        {
            Destination = destination;
        }

    }
}