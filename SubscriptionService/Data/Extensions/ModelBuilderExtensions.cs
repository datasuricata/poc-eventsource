using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SubscriptionService.Data
{
    public static class ModelBuilderExtensions
    {
        public static void MapFromAssembly(this ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public static void UnDeleteCascade(this ModelBuilder builder)
        {

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                entityType.GetForeignKeys().Where(mutable => !mutable.IsOwnership && mutable.DeleteBehavior == DeleteBehavior.Cascade).ToList()
                    .ForEach(mutable => mutable.DeleteBehavior = DeleteBehavior.Restrict);
            }
        }
    }
}
