using System;

namespace Domain.Entities
{
    public abstract class Entity
    {
        protected delegate void Assert(bool error, string message);

        public Entity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTimeOffset.Now;
        }

        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
