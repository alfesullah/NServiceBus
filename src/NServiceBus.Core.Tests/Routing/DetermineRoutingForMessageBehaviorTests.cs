namespace NServiceBus.Core.Tests.Routing
{
    using System.Collections.Generic;
    using NServiceBus.ConsistencyGuarantees;
    using NServiceBus.Pipeline.Contexts;
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

            var context = new OutgoingContext(null,null,typeof(MyMessage), null,options);

            behavior.Invoke(context,()=>{});

            var routingStrategy = (DirectRoutingStrategy)context.Get<RoutingStrategy>();

            routingStrategy.Dispatch(new OutgoingMessage("some id",new Dictionary<string, string>(),null ),new NoConsistencyRequired(), new List<DeliveryConstraint>());

            Assert.AreEqual("destination endpoint",fakeSender.TransportSendOptions.Destination);
        }

        [Test]
        public void Should_route_to_local_endpoint_if_requested_so()
        {
            var fakeSender = new FakeSender();

            var behavior = InitializeBehavior(fakeSender,"MyLocalAddress");
            var options = new SendOptions();

            options.RouteToLocalEndpointInstance();

            var context = new OutgoingContext(null, null, typeof(MyMessage), null, options);

            behavior.Invoke(context, () => { });

            var routingStrategy = (DirectRoutingStrategy)context.Get<RoutingStrategy>();

            routingStrategy.Dispatch(new OutgoingMessage("some id", new Dictionary<string, string>(), null), new NoConsistencyRequired(), new List<DeliveryConstraint>());

            Assert.AreEqual("MyLocalAddress", fakeSender.TransportSendOptions.Destination);
        }

        static DetermineRoutingForMessageBehavior InitializeBehavior(ISendMessages sender = null,string localAddress = null)
        {
            return new DetermineRoutingForMessageBehavior(sender,localAddress);
        }

        class FakeSender:ISendMessages
        {
            public TransportSendOptions TransportSendOptions { get; set; }

            public void Send(OutgoingMessage message, TransportSendOptions sendOptions)
            {
                TransportSendOptions = sendOptions;
            }
        }

        class MyMessage
        { }
    }
}