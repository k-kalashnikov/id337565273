using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SP.FileStorage.Client.Services;
using SP.Messenger.Persistence;

namespace SP.Messenger.Hub.Service.AssemblyMigrations
{
    public class CreateBucketsMigration : AssemblyMigration
    {
        private IFileStorageClientService _fileStorageClientService;

        public CreateBucketsMigration(IServiceProvider serviceProvider)
            :base(serviceProvider)
        {
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetService<MessengerDbContext>();
            _fileStorageClientService = scope.ServiceProvider.GetService<IFileStorageClientService>();
            
            var chats = await context.Chats.ToArrayAsync(cancellationToken);
            
            foreach (var chat in chats)
            {
                var existBucket = await _fileStorageClientService
                    .CheckExistBucketAsync(chat.Name, cancellationToken);
                
                if (existBucket) continue;
                var result = await _fileStorageClientService.CreateBucketAsync(chat.Name, cancellationToken);
                Log.Information(result.Data
                    ? $"**** bucket{chat.Name} = created"
                    : $"**** bucket{chat.Name} = NOT created, reason: {result.Errors}");
            }
        }
    }
}