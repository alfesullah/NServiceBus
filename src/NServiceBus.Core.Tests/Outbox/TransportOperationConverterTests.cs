namespace NServiceBus.Core.Tests.Outbox
{
    using NServiceBus.Outbox;
    using NServiceBus.Unicast;
    using NUnit.Framework;

    [TestFixture]
    class TransportOperationConverterTests
    {
        [Test]
        public void DeliveryOptions()
        {
            var options = new DeliveryMessageOptions
            {
                NonDurable = true
            };

            var converted = options.ToTransportOperationOptions().ToDeliveryOptions();

            Assert.AreEqual(converted.NonDurable, options.NonDurable);
      
        }

     
        class MyMessage
        { }
    }
}
