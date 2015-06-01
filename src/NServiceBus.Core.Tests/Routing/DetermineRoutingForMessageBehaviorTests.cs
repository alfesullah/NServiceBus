namespace NServiceBus.Core.Tests.Routing
{
    using System;
    using System.Collections.Generic;
    using NServiceBus.ConsistencyGuarantees;
    using NServiceBus.DeliveryConstraints;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Routing;
    using NServiceBus.Transports;
    using NUnit.Framework;

    [TestFixture]
    public class DetermineRoutingForMessageBehaviorTests
    {
        [Test]
        public void Should_use_explicit_route_for_sends_if_present()
        {
            var fakeSender = new FakeSender();

            var behavior = InitializeBehavior(fakeSender);
            var options = new SendOptions();

            options.SetDestination("destination endpoint");

            var context = new OutgoingContext(null, null, typeof(MyMessage), null, options);

            behavior.Invoke(context, () => { });

            var routingStrategy = (DirectRoutingStrategy)context.Get<RoutingStrategy>();

            routingStrategy.Dispatch(new OutgoingMessage("some id", new Dictionary<string, string>(), null), new NoConsistencyRequired(), new List<DeliveryConstraint>(),null);

            Assert.AreEqual("destination endpoint", fakeSender.TransportSendOptions.Destination);
        }

        [Test]
        public void Should_route_to_local_endpoint_if_requested_so()
        {
            var fakeSender = new FakeSender();

            var behavior = InitializeBehavior(fakeSender, "MyLocalAddress");
            var options = new SendOptions();

            options.RouteToLocalEndpointInstance();

            var context = new OutgoingContext(null, null, typeof(MyMessage), null, options);

            behavior.Invoke(context, () => { });

            var routingStrategy = (DirectRoutingStrategy)context.Get<RoutingStrategy>();

            routingStrategy.Dispatch(new OutgoingMessage("some id", new Dictionary<string, string>(), null), new NoConsistencyRequired(), new List<DeliveryConstraint>(), null);

            Assert.AreEqual("MyLocalAddress", fakeSender.TransportSendOptions.Destination);
        }

        [Test]
        public void Should_route_using_the_mappings_if_no_destination_is_set()
        {
            var fakeSender = new FakeSender();

            var router = new FakeRouter();

            var behavior = InitializeBehavior(fakeSender, router: router);
            var options = new SendOptions();

            var context = new OutgoingContext(null, null, typeof(MyMessage), null, options);

            behavior.Invoke(context, () => { });

            var routingStrategy = (DirectRoutingStrategy)context.Get<RoutingStrategy>();

            routingStrategy.Dispatch(new OutgoingMessage("some id", new Dictionary<string, string>(), null), new NoConsistencyRequired(), new List<DeliveryConstraint>(), null);

            Assert.AreEqual("MappedDestination", fakeSender.TransportSendOptions.Destination);
        }

        static DetermineRoutingForMessageBehavior InitializeBehavior(ISendMessages sender = null, string localAddress = null, MessageRouter router = null)
        {
            return new DetermineRoutingForMessageBehavior(sender, localAddress, router);
        }

        class FakeSender : ISendMessages
        {
            public TransportSendOptions TransportSendOptions { get; set; }

            public void Send(OutgoingMessage message, TransportSendOptions sendOptions)
            {
                TransportSendOptions = sendOptions;
            }
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