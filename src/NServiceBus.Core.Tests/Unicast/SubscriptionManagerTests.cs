namespace NServiceBus.Unicast.Tests
{
    using System.Threading;
    using NServiceBus.Transports;
    using NServiceBus.Unicast.Subscriptions.MessageDrivenSubscriptions;
    using NUnit.Framework;

    [TestFixture]
    public class SubscriptionManagerTests
    {
        [Test]
        public void Should_send_the_assemblyQualified_name_as_subscription_type()
        {
            var sender = new FakeSender();

            var subscriptionManager = new SubscriptionManager("subscriber", sender);


            subscriptionManager.Subscribe(typeof(TestEvent),"publish");

            sender.MessageAvailable.WaitOne();
            Assert.AreEqual(typeof(TestEvent).AssemblyQualifiedName,sender.MessageSent.Headers[Headers.SubscriptionMessageType] );
        }

        class FakeSender:IDispatchMessages
        {
            public FakeSender()
            {
                MessageAvailable = new AutoResetEvent(false);
            }


            public OutgoingMessage MessageSent { get; private set; }

            public DispatchOptions SendOptions { get; private set; }

            public AutoResetEvent MessageAvailable { get; private set; }

            public void Dispatch(OutgoingMessage message, DispatchOptions dispatchOptions)
            {
                MessageSent = message;

                SendOptions = dispatchOptions;

                MessageAvailable.Set();
            }
        }

        class TestEvent
        {
            
        }
    }
}