namespace NServiceBus.Unicast.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Message metadata class.
    /// </summary>
    public partial class MessageMetadata
    {
        
        internal MessageMetadata(Type messageType = null, bool recoverable = false, IEnumerable<Type> messageHierarchy = null)
        {
            MessageType = messageType;
            Recoverable = recoverable;
            MessageHierarchy = (messageHierarchy == null ? new List<Type>() : new List<Type>(messageHierarchy)).AsReadOnly();
        }

        /// <summary>
        /// The <see cref="Type"/> of the message instance.
        /// </summary>
        public Type MessageType { get; private set; }

        /// <summary>
        ///     Gets whether or not the message is supposed to be guaranteed deliverable.
        /// </summary>
        public bool Recoverable { get; private set; }

        /// <summary>
        /// The message instance hierarchy.
        /// </summary>
        public IEnumerable<Type> MessageHierarchy { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("MessageType: {0}, Recoverable: {1}, Parent types: {2}", MessageType, Recoverable,
                string.Join(";", MessageHierarchy.Select(pt => pt.FullName)));
        }
    }
}