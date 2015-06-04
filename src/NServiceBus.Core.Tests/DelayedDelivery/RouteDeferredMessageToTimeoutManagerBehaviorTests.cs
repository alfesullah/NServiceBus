namespace NServiceBus.Core.Tests.Timeout
{
    using System;
    using NServiceBus.DelayedDelivery;
    using NServiceBus.DeliveryConstraints;
    using NServiceBus.Pipeline.Contexts;
    using NServiceBus.Routing;
    using NServiceBus.Timeout;
    using NUnit.Framework;

    class RouteDeferredMessageToTimeoutManagerBehaviorTests
    {
        [Test]
        public void Should_reroute_to_tm()
        {
            var behavior = new RouteDeferredMessageToTimeoutManagerBehavior("tm");
            var delay = TimeSpan.FromDays(1);

            var context = new OutgoingContext(null, null, null, new SendOptions());
            context.AddDeliveryConstraint(new DelayDeliveryWith(delay));
            context.Set<RoutingStrategy>(new DirectToTargetDestination("target"));

            behavior.Invoke(context, () => { });

            Assert.AreEqual("tm",((DirectToTargetDestination)context.Get<RoutingStrategy>()).Destination);

            context.AssertHeaderWasSet(TimeoutManagerHeaders.RouteExpiredTimeoutTo, h => h == "target");
        }


        [Test]
        public void Delayed_delivery_using_the_tm_is_only_supported_for_sends()
        {
            var behavior = new RouteDeferredMessageToTimeoutManagerBehavior("tm");
            var delay = TimeSpan.FromDays(1);

            var context = new OutgoingContext(null, null, null, new SendOptions());
            context.AddDeliveryConstraint(new DelayDeliveryWith(delay));
            context.Set<RoutingStrategy>(new ToAllSubscribers(null));

            var ex = Assert.Throws<Exception>(()=> behavior.Invoke(context, () => { }));

            Assert.True(ex.Message.Contains("Direct routing"));
        }
        [Test]
        public void Should_set_the_expiry_header_to_a_absolute_utc_time_calculated_based_on_delay()
        {
            var behavior = new RouteDeferredMessageToTimeoutManagerBehavior("tm");
            var delay = TimeSpan.FromDays(1);

            var context = new OutgoingContext(null,null,null,new SendOptions());
            context.AddDeliveryConstraint(new DelayDeliveryWith(delay));
            context.Set<RoutingStrategy>(new DirectToTargetDestination("target"));
            
            behavior.Invoke(context,()=>{});

            context.AssertHeaderWasSet(TimeoutManagerHeaders.Expire, h => DateTimeExtensions.ToUtcDateTime(h) <= DateTime.UtcNow + delay);
        }

        [Test]
        public void Should_set_the_expiry_header_to_a_absolute_utc_time()
        {
            var behavior = new RouteDeferredMessageToTimeoutManagerBehavior("tm");
            var at = DateTime.UtcNow + TimeSpan.FromDays(1);

            var context = new OutgoingContext(null, null, null, new SendOptions());
            context.AddDeliveryConstraint(new DoNotDeliverBefore(at));
            context.Set<RoutingStrategy>(new DirectToTargetDestination("target"));

            behavior.Invoke(context, () => { });

            context.AssertHeaderWasSet(TimeoutManagerHeaders.Expire, h => h == DateTimeExtensions.ToWireFormattedString(at));
        }
    }
}