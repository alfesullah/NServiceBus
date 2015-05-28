namespace NServiceBus.Core.Tests.Routing
{
    using System.Collections.Generic;
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

            var behavior = InitializeBehavior(sender:fakeSender);
            var options = new SendOptions();

            options.SetDestination("destination endpoint");

            var context = new OutgoingContext(null,null,typeof(MyMessage), null,options);

            behavior.Invoke(context,()=>{});

            var routingStrategy = (DirectRoutingStrategy)context.Get<RoutingStrategy>();

            routingStrategy.Dispatch(new OutgoingMessage("some id",new Dictionary<string, string>(),null ),new NoConsistencyRequired(), new List<DeliveryConstraint>());

            Assert.AreEqual("destination endpoint",fakeSender.TransportSendOptions.Destination);
        }

        static DetermineRoutingForMessageBehavior InitializeBehavior(ISendMessages sender = null)
        {
            var apiFactory = new TransportApiFactory(sender);
            return new DetermineRoutingForMessageBehavior(apiFactory);
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