using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SP.Messenger.Persistence;

namespace SP.Messenger.Application.Test.Infrastructure
{
    public class TestPopstgresBase
    {
        public MessengerDbContext GetDbContext(bool initialize = false)
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();

            var connectionString = config["ConnectionStrings:MessengerDatabase"];
            var builder = new DbContextOptionsBuilder<MessengerDbContext>();
            builder.UseNpgsql(connectionString);

            var context = new MessengerDbContext(builder.Options);

            if (initialize)
            {
                MessengerInitializer.InitializeAsync(context);
            }

            return context;
        }
    }
}
