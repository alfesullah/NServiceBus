namespace NServiceBus
{
    using System;
    using NServiceBus.Performance.TimeToBeReceived;
    using NServiceBus.Pipeline;
    using NServiceBus.Pipeline.Contexts;

    class ApplyTimeToBeReceivedBehavior:Behavior<OutgoingContext>
    {
        public ApplyTimeToBeReceivedBehavior(TimeToBeReceivedMappings timeToBeReceivedMappings)
        {
            this.timeToBeReceivedMappings = timeToBeReceivedMappings;
        }
        
        public override void Invoke(OutgoingContext context, Action next)
        {
            TimeSpan timeToBeReceived;

            if (timeToBeReceivedMappings.TryGetTimeToBeReceived(context.MessageType, out timeToBeReceived))
            {
                context.AddDeliveryConstraint(new DiscardIfNotReceivedBefore(timeToBeReceived));
                context.SetHeader(Headers.TimeToBeReceived, timeToBeReceived.ToString());
            }

            next();
        }

        readonly TimeToBeReceivedMappings timeToBeReceivedMappings;

        
        //if (!options.TimeToBeReceived.HasValue)
            //{
            //    if (messageDefinitions.TimeToBeReceived < TimeSpan.MaxValue)
            //    {
            //        options.TimeToBeReceived = messageDefinitions.TimeToBeReceived;
            //    }
            //} 

        //[TestFixture]
        //public class When_postponing_delivery
        //{
        //    [Test]
        //    public void Should_throw_for_TimeToBeReceived_set()
        //    {
        //        var invalidOperationException = Assert.Throws<InvalidOperationException>(() => new Validations(new Conventions()).AssertIsValidForPostponedDelivery(typeof(MyDeferredMessage)));
        //        Assert.AreEqual("Postponed delivery of messages with TimeToBeReceived set is not supported. Remove the TimeToBeReceived attribute to postpone messages of this type.", invalidOperationException.Message);
        //    }

        //    [Test]
        //    public void Should_not_throw_for_TimeToBeReceived_no_set()
        //    {
        //        new Validations(new Conventions()).AssertIsValidForPostponedDelivery(typeof(MyMessage));
        //    }
        //}

        //todo: move into the defer behavior
        //public void AssertIsValidForPostponedDelivery(Type messageType)
        //{
        //    //if (conventions.GetTimeToBeReceived(messageType) < TimeSpan.MaxValue)
        //    //{
        //    //    throw new InvalidOperationException("Postponed delivery of messages with TimeToBeReceived set is not supported. Remove the TimeToBeReceived attribute to postpone messages of this type.");
        //    //}
        //}
    }

    
      //[Test]
      //  public void Should_use_TimeToBeReceived_from_bottom_of_tree()
      //  {
      //      var conventions = new NServiceBus.Conventions();
      //      var timeToBeReceivedAction = conventions.GetTimeToBeReceived(typeof(InheritedClassWithAttribute));
      //      Assert.AreEqual(TimeSpan.FromSeconds(2), timeToBeReceivedAction);
      //  }

      //  [Test]
      //  public void Should_use_inherited_TimeToBeReceived()
      //  {
      //      var conventions = new NServiceBus.Conventions();
      //      var timeToBeReceivedAction = conventions.GetTimeToBeReceived(typeof(InheritedClassWithNoAttribute));
      //      Assert.AreEqual(TimeSpan.FromSeconds(1), timeToBeReceivedAction);
      //  }

      //  [TimeToBeReceivedAttribute("00:00:01")]
      //  class BaseClass
      //  {
      //  }

      //  [TimeToBeReceivedAttribute("00:00:02")]
      //  class InheritedClassWithAttribute : BaseClass
      //  {

      //  }

      //  class InheritedClassWithNoAttribute : BaseClass
      //  {

      //  }

}