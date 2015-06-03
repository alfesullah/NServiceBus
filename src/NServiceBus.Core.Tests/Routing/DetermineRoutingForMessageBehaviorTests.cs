namespace NServiceBus.Core.Tests.Routing
{
    using System;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Routing;
    using NUnit.Framework;

    [TestFixture]
    public class DetermineRoutingForMessageBehaviorTests
    {
        [Test]
        public void Should_use_explicit_route_for_sends_if_present()
        {
            var behavior = InitializeBehavior();
            var options = new SendOptions();

            options.SetDestination("destination endpoint");

            var context = new OutgoingContext(null, null, typeof(MyMessage), null, options);

            behavior.Invoke(context, () => { });

            var routingStrategy = (DirectToTargetDestination)context.Get<RoutingStrategy>();

            Assert.AreEqual("destination endpoint", routingStrategy.Destination);
        }

        [Test]
        public void Should_route_to_local_endpoint_if_requested_so()
        {
             var behavior = InitializeBehavior("MyLocalAddress");
            var options = new SendOptions();

            options.RouteToLocalEndpointInstance();

            var context = new OutgoingContext(null, null, typeof(MyMessage), null, options);

            behavior.Invoke(context, () => { });

            var routingStrategy = (DirectToTargetDestination)context.Get<RoutingStrategy>();


            Assert.AreEqual("MyLocalAddress", routingStrategy.Destination);
        }

        [Test]
        public void Should_route_using_the_mappings_if_no_destination_is_set()
        {
            var router = new FakeRouter();

            var behavior = InitializeBehavior(router: router);
            var options = new SendOptions();

            var context = new OutgoingContext(null, null, typeof(MyMessage), null, options);

            behavior.Invoke(context, () => { });

            var routingStrategy = (DirectToTargetDestination)context.Get<RoutingStrategy>();

            Assert.AreEqual("MappedDestination", routingStrategy.Destination);
        }

        [Test]
        public void Should_use_to_all_subscribers_strategy_for_events()
        {
            var behavior = InitializeBehavior();
            var options = new PublishOptions();

            var context = new OutgoingContext(null, null, typeof(MyMessage), null, options);

            behavior.Invoke(context, () => { });

            var routingStrategy = (ToAllSubscribers)context.Get<RoutingStrategy>();

            Assert.AreEqual(typeof(MyMessage), routingStrategy.EventType);
        }

        static DetermineRoutingForMessageBehavior InitializeBehavior(string localAddress = null, MessageRouter router = null)
        {
            return new DetermineRoutingForMessageBehavior(localAddress, router);
        }

        class MyMessage
        { }
        class FakeRouter : MessageRouter
        {
            public override bool TryGetRoute(Type messageType, out string destination)
            {
                if (messageType == typeof(MyMessage))
                {
                    destination= "MappedDestination";

                    return true;
                }

                destination = null;
                return false;
            }
        }
    }
}