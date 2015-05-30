namespace NServiceBus
{
    using System.Collections.Generic;
    using NServiceBus.ConsistencyGuarantees;
    using NServiceBus.Pipeline;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Transports;

    class DispatchMessageToTransportTerminator : PipelineTerminator<PhysicalOutgoingContextStageBehavior.Context>
    {
        public override void Terminate(PhysicalOutgoingContextStageBehavior.Context context)
        {
            var state = context.Extensions.GetOrCreate<State>();
            state.Headers[Headers.MessageId] = state.MessageId;

            var message = new OutgoingMessage(state.MessageId, state.Headers, context.Body);
            
            var requiredGuarantee = context.GetConsistencyGuarantee();

            var deliveryConstraints = context.GetDeliveryConstraints();
      
            context.Get<RoutingStrategy>()
                .Dispatch(message, requiredGuarantee,deliveryConstraints);
        }
     
        public class State
        {
            public State()
            {
                Headers = new Dictionary<string, string>();
                MessageId = CombGuid.Generate().ToString();
            }
            public Dictionary<string, string> Headers { get; private set; }
            public string MessageId { get; set; }
        }
    }
}