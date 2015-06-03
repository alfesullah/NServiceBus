namespace NServiceBus
{
    class DirectRoutingStrategy : RoutingStrategy
    {
        public string Destination { get; private set; }

        public DirectRoutingStrategy(string destination)
        {
            Destination = destination;
        }

    }
}