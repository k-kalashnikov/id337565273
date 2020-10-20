using System;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Hub.Service.AssemblyMigrations
{
    public abstract class AssemblyMigration : IAssemblyMigration
    {
        public IServiceProvider ServiceProvider { get; }

        protected AssemblyMigration(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
        public abstract Task DoWork(CancellationToken cancellationToken);
    }
}