using System;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Hub.Service.AssemblyMigrations
{
	public interface IAssemblyMigration
	{
		IServiceProvider ServiceProvider { get; }

		Task DoWork(CancellationToken cancellationToken);
	}
}
