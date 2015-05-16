namespace NServiceBus.Core.Tests.DataBus
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Unicast;
    using NUnit.Framework;
    using Conventions = NServiceBus.Conventions;
    using SendOptions = NServiceBus.SendOptions;

    [TestFixture]
    class When_applying_the_databus_message_mutator_to_null_properties 
    {
        [Test]
        public void Should_not_blow_up()
        {
            var context = new OutgoingContext(null,new SendMessageOptions("MyEndpoint"), null, new MessageWithNullDataBusProperty(),new SendOptions());
            var sendBehavior = new DataBusSendBehavior
            {
                DataBus = null,
                Conventions = new Conventions(),
                DataBusSerializer = new DefaultDataBusSerializer(),
            };
            
            using (var stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, "test");
                stream.Position = 0;

                sendBehavior.Invoke(context, () => { });            
            }
        }

    }
}