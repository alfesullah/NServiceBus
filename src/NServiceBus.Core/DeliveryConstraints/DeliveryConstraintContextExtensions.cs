namespace NServiceBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NServiceBus.DeliveryConstraints;
    using NServiceBus.Pipeline.Contexts;

    static class DeliveryConstraintContextExtensions
    {
        public static void AddDeliveryConstraint(this OutgoingContext context, DeliveryConstraint constraint)
        {
            List<DeliveryConstraint> constraints;

            if (!context.TryGet(out constraints))
            {
                constraints = new List<DeliveryConstraint>();

                context.Set(constraints);
            }

            if (constraints.Any(c => c.GetType() == constraint.GetType()))
            {
                throw new InvalidOperationException("Constraint of type " + constraint.GetType().FullName + " already exists");
            }

            constraints.Add(constraint);
        }

        public static IEnumerable<DeliveryConstraint> GetDeliveryConstraints(this PhysicalOutgoingContextStageBehavior.Context context)
        {
            List<DeliveryConstraint> constraints;

            if (context.TryGet(out constraints))
            {
                return constraints;
            }

            return new List<DeliveryConstraint>();
        }

        public static bool TryGet<T>(this IEnumerable<DeliveryConstraint> list,out T constraint) where T : class
        {
            constraint = list.SingleOrDefault(c => c is T) as T;

            return constraint != null;
        }
    }
}