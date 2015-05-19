namespace NServiceBus
{
    using System.Collections.Generic;
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

            option.Extensions.GetOrCreate<RouteMessageBehavior.State>()
                .Destination = destination;
        }

        /// <summary>
        /// Allows a specific physical address to be used to route this message
        /// </summary>
        /// <param name="option">Option beeing extended</param>
        /// <param name="destination">The destination address</param>
        public static void SetDestination(this ReplyOptions option, string destination)
        {
            Guard.AgainstNullAndEmpty(destination, "destination");

            option.Extensions.GetOrCreate<RouteMessageBehavior.State>()
                .Destination = destination;
        }

        /// <summary>
        /// Routes this message to the local endpoint instance
        /// </summary>
        /// <param name="option">Context beeing extended</param>
        public static void RouteToLocalEndpointInstance(this SendOptions option)
        {
            option.Extensions.GetOrCreate<RouteMessageBehavior.State>()
                .RouteToLocalInstance = true;
        }
    }

    abstract class RoutingStrategy
    {
        public void Dehydrate(Dictionary<string, string> options)
        {
            

        }

        public abstract void Dispatch(OutgoingMessage message);
    }

    class RoutingStrategyFactory
    {
        public RoutingStrategy Create(Dictionary<string, string> options)
        {
            return null;
        }
    }
}