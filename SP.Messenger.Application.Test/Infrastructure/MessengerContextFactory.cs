using Microsoft.EntityFrameworkCore;
using SP.Messenger.Persistence;
using System;

namespace SP.Messenger.Application.Test.Infrastructure
{
    public class MessengerContextFactory
    {
        public static MessengerDbContext Create()
        {
            var options = new DbContextOptionsBuilder<MessengerDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new MessengerDbContext(options);
            context.Database.EnsureCreated();

            MessengerInitializer.InitializeAsync(context);

            return context;
        }
        public static void Destroy(MessengerDbContext context)
        {
            context.Database.EnsureDeleted();

            context.Dispose();
        }
    }
}
