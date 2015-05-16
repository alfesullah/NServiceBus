namespace NServiceBus.Pipeline.Contexts
{
    using System;
    using NServiceBus.Extensibility;
    using NServiceBus.Unicast;

    /// <summary>
    /// 
    /// </summary>
    public abstract class PhysicalOutgoingContextStageBehavior : Behavior<PhysicalOutgoingContextStageBehavior.Context>
    {
        /// <summary>
        /// The <see cref="BehaviorContext"/> for <see cref="PhysicalOutgoingContextStageBehavior"/>.
        /// </summary>
        public class Context : BehaviorContext
        {

            /// <summary>
            /// Initializes an instance of <see cref="Context"/>.
            /// </summary>
            public Context(byte[] body, OutgoingContext parentContext)
                : base(parentContext)
            {
                Body = body;
                DeliveryMessageOptions = parentContext.DeliveryMessageOptions;
                MessageType = parentContext.MessageType;
                Extensions = parentContext.Extensions;
            }

            /// <summary>
            /// The logical message type
            /// </summary>
            public Type MessageType { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            public DeliveryMessageOptions DeliveryMessageOptions { get; private set; }

            /// <summary>
            /// A <see cref="byte"/> array containing the serialized contents of the outgoing message.
            /// </summary>
            public byte[] Body { get; set; }

            /// <summary>
            /// Place for extensions to store their data
            /// </summary>
            public OptionExtensionContext Extensions { get; private set; }
        }
    }
}