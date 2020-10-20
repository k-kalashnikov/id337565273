using Microsoft.EntityFrameworkCore;
using SP.Messenger.Persistence.Infrastructure;

namespace SP.Messenger.Persistence
{
    public class MessengerDbContextFactory : DesignTimeDbContextFactoryBase<MessengerDbContext>
    {
        protected override MessengerDbContext CreateNewInstance(DbContextOptions<MessengerDbContext> options)
            => new MessengerDbContext(options);
    }
}
