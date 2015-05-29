namespace NServiceBus
{
    using System;
    using System.Collections.Generic;
    using NServiceBus.ConsistencyGuarantees;
    using NServiceBus.Extensibility;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Transports;

    /// <summary>
    /// 
    /// </summary>
    public static class RoutingContextExtensions
    {
        /// <summary>
        /// Tells if this operation is a reply
        /// </summary>
        /// <param name="context">Context beeing extended</param>
        /// <returns>True if the operation is a reply</returns>
        public static bool IsReply(this OutgoingContext context)
        {
            return context.Get<ExtendableOptions>() is ReplyOptions;
        }

        /// <summary>
        /// Tells if this operation is a publish
        /// </summary>
        /// <param name="context">Context beeing extended</param>
        /// <returns>True if the operation is a publish</returns>
        public static bool IsPublish(this OutgoingContext context)
        {
            return context.Get<ExtendableOptions>() is PublishOptions;
        }

        /// <summary>
        /// Tells if this operation is a publish
        /// </summary>
        /// <param name="context">Context beeing extended</param>
        /// <returns>True if the operation is a publish</returns>
        public static bool IsPublish(this PhysicalOutgoingContextStageBehavior.Context context)
        {
            return context.Get<ExtendableOptions>() is PublishOptions;
        }

        /// <summary>
        /// Tells if this operation is a send
        /// </summary>
        /// <param name="context">Context beeing extended</param>
        /// <returns>True if the operation is a publish</returns>
        public static bool IsSend(this OutgoingContext context)
        {
            return context.Get<ExtendableOptions>() is SendOptions || context.Get<ExtendableOptions>() is SendLocalOptions;
        }
        /// <summary>
        /// Tells if this operation is a reply
        /// </summary>
        /// <param name="context">Context beeing extended</param>
        /// <returns>True if the operation is a reply</returns>
        public static bool IsReply(this PhysicalOutgoingContextStageBehavior.Context context)
        {
            return context.Get<ExtendableOptions>() is ReplyOptions;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public static class RoutingOptionExtensions
    {
        /// <summary>
        /// Allows a specific physical address to be used to route this message
        /// </summary>
        /// <param name="option">Option beeing extended</param>
        /// <param name="destination">The destination address</param>
        public static void SetDestination(this SendOptions option,string destination)
        {
            Guard.AgainstNullAndEmpty(destination,"destination");

            option.Extensions.GetOrCreate<DetermineRoutingForMessageBehavior.State>()
                .ExplicitDestination = destination;
        }

        /// <summary>
        /// Allows the target endpoint instance for this reply to set. If not used the reply will be sent to the `ReplyToAddress` of the incoming message
        /// </summary>
        /// <param name="option">Option beeing extended</param>
        /// <param name="destination">The new target address</param>
        public static void OverrideReplyToAddressOfIncomingMessage(this ReplyOptions option, string destination)
        {
            Guard.AgainstNullAndEmpty(destination, "destination");

            option.Extensions.GetOrCreate<DetermineRoutingForMessageBehavior.State>()
                .ExplicitDestination = destination;
        }

        /// <summary>
        /// Routes this message to the local endpoint instance
        /// </summary>
        /// <param name="option">Context beeing extended</param>
        public static void RouteToLocalEndpointInstance(this SendOptions option)
        {
            option.Extensions.GetOrCreate<DetermineRoutingForMessageBehavior.State>()
                .RouteToLocalInstance = true;
        }
    }

    abstract class RoutingStrategy
    {
        public void Deserialize(Dictionary<string, string> options)
        {
            

        }

        public abstract void Dispatch(OutgoingMessage message,
            ConsistencyGuarantee mimimumConsistencyGuarantee,
            List<DeliveryConstraint> constraints);
    }

    /// <summary>
    /// 
    /// </summary>
    public class DiscardIfNotReceivedBefore : DeliveryConstraint
    {
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan MaxTime { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxTime"></param>
        public DiscardIfNotReceivedBefore(TimeSpan maxTime)
        {
            MaxTime = maxTime;
        }

        internal override bool Deserialize(Dictionary<string, string> options)
        {
            throw new NotImplementedException();
        }

        internal override void Serialize(Dictionary<string, string> options)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class DelayedDelivery : DeliveryConstraint
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="delay"></param>
        public DelayedDelivery(TimeSpan delay)
        {
            DelayDeliveryWith = delay;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doNotDeliverBefore"></param>
        public DelayedDelivery(DateTime doNotDeliverBefore)
        {
            DoNotDeliverBefore = doNotDeliverBefore;
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan? DelayDeliveryWith { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public DateTime? DoNotDeliverBefore { get; private set; }

        internal override bool Deserialize(Dictionary<string, string> options)
        {
            throw new NotImplementedException();
        }

        internal override void Serialize(Dictionary<string, string> options)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class DeliveryConstraint
    {
        //public bool NonDurable { get; set; }
       
        
        //public TimeSpan? TimeToBeReceived{ get; set; }


        internal abstract bool Deserialize(Dictionary<string, string> options);

        internal abstract void Serialize(Dictionary<string, string> options);
    }

    /// <summary>
    /// 
    /// </summary>
    public class NonDurableDelivery : DeliveryConstraint
    {
        internal override bool Deserialize(Dictionary<string, string> options)
        {
            throw new NotImplementedException();
        }

        internal override void Serialize(Dictionary<string, string> options)
        {
            throw new NotImplementedException();
        }
    }

    class RoutingStrategyFactory
    {
        public RoutingStrategy Create(Dictionary<string, string> options)
        {
            return null;
        }
    }
}