using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SubscriptionService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Client> Client { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.UnDeleteCascade();
            builder.MapFromAssembly();

            base.OnModelCreating(builder);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var entry in ChangeTracker.Entries().Where(entry =>
                            entry.Entity.GetType().GetProperty(nameof(Entity.CreatedAt)) != null))
            {
                if (entry.Property(nameof(Entity.CreatedAt)) != null)
                    if (entry.State == EntityState.Added)
                        entry.Property(nameof(Entity.CreatedAt)).CurrentValue = DateTimeOffset.Now;
                    else if (entry.State == EntityState.Modified)
                        entry.Property(nameof(Entity.CreatedAt)).IsModified = false;
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}