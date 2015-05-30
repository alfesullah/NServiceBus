namespace NServiceBus
{
    using System;
    using System.Collections.Generic;

    class TimeToBeReceivedMappings
    {
        public TimeToBeReceivedMappings(Dictionary<Type, TimeSpan> mappings)
        {
            this.mappings = mappings;
        }
        
        public bool TryGetTimeToBeReceived(Type messageType, out TimeSpan timeToBeReceived)
        {
            return mappings.TryGetValue(messageType, out timeToBeReceived);
        }

      
        Dictionary<Type, TimeSpan> mappings;
    }
}