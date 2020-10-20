using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SP.Accounts.Client.Interfaces;
using SP.Messenger.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Hub.Service.AssemblyMigrations
{
	public class GPM6210 : IAssemblyMigration
	{
		public IServiceProvider ServiceProvider { get; set; }
		private MessengerDbContext _context;
		private IAccountsClientsService _accountsClient;

		private List<string> _accounts = new List<string>()
		{
			"bk_centr@mail.ru",
			"parazutdinova@betokam.ru"
		};

		public GPM6210(IServiceProvider serviceProvider)
		{
			ServiceProvider = serviceProvider;
		}

		public async Task DoWork(CancellationToken cancellationToken)
		{
			Log.Information($"{nameof(GPM6210)} invoked");

			using (var scope = ServiceProvider.CreateScope())
			{
				_context = scope.ServiceProvider.GetRequiredService<MessengerDbContext>();
				_accountsClient = scope.ServiceProvider.GetRequiredService<IAccountsClientsService>();


				var accounts = await _accountsClient.GetAccountsByEmail(_accounts, cancellationToken);

				foreach (var item in accounts)
				{
					Log.Information($"{nameof(GPM6210)} {item.Email} {item.Id}");

				}
			}
		}
	}
}
