namespace NServiceBus.Core.Tests.Serializers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Serialization;
    using NServiceBus.Unicast;
    using NServiceBus.Unicast.Messages;
    using NUnit.Framework;
    using Conventions = NServiceBus.Conventions;
    using SendOptions = NServiceBus.SendOptions;

    public class SerializeMessagesBehaviorTests
    {
        [Test]
        public void Should_set_content_type_header()
        {
            var registry = new MessageMetadataRegistry(true,new Conventions());

            registry.RegisterMessageType(typeof(MyMessage));

            var behavior = new SerializeMessagesBehavior(new FakeSerializer("myContentType"),registry);

            var context = new OutgoingContext(null, new SendMessageOptions(), typeof(MyMessage), null, new SendOptions());

            behavior.Invoke(context, c =>
            {
                
            });

            Assert.AreEqual("myContentType", context.Extensions.GetOrCreate<DispatchMessageToTransportTerminator.State>().Headers[Headers.ContentType]);
        }

        public class FakeSerializer : IMessageSerializer
        {
            public FakeSerializer(string contentType)
            {
                ContentType = contentType;
            }

            public void Serialize(object message, Stream stream)
            {
                
            }

            public object[] Deserialize(Stream stream, IList<Type> messageTypes = null)
            {
                throw new NotImplementedException();
            }

            public string ContentType { get; private set; }
        }

        class MyMessage { }
    }
}