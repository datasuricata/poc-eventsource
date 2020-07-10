using Domain.Entities;
using System;

namespace Domain.Events
{
    public class OrderCreatedEvent
    {
        public OrderCreatedEvent(Guid id, Subscription subscription)
        {
            Id = id;
            Subscription = subscription;
        }

        public Guid Id { get; set; }
        public Subscription Subscription { get; set; }
    }
}
