using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nest;
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
	public class GPM5903 : IAssemblyMigration
	{
		public IServiceProvider ServiceProvider { get; set; }
		private IAccountsClientsService _accountsService;
		private MessengerDbContext _context;

		public GPM5903(IServiceProvider serviceProvider)
		{
			ServiceProvider = serviceProvider;
		}

		public async Task DoWork(CancellationToken cancellationToken)
		{
			Log.Information($"{nameof(GPM5903)} invoked");

			using (var scope = ServiceProvider.CreateScope())
			{
				_context = scope.ServiceProvider.GetRequiredService<MessengerDbContext>();
				_accountsService = scope.ServiceProvider.GetRequiredService<IAccountsClientsService>();

				var chats = await _context.Chats
					.Where(m => m.Name.Contains("ООО «ТеплоЭнергоСистемы»"))
					.ToListAsync(cancellationToken);

				if (chats == null)
				{
					Log.Error($"{nameof(GPM5903)} Chats not found");
					return;
				}

				Log.Information($"{nameof(GPM5903)} Found chats {chats?.Count() ?? 0} {string.Join(',', chats.Select(c => c.ChatId.ToString()))}");

				var accounts = await _accountsService.GetAccountsByOrganizationId(439248, cancellationToken);

				if (accounts == null)
				{
					Log.Error($"{nameof(GPM5903)} accounts not found");
					return;
				}

				Log.Information($"{nameof(GPM5903)} Found accounts {accounts?.Count() ?? 0} {string.Join(',', accounts.Select(c => c.Id.ToString()))}");


				var dbAccounts = await _context.Accounts
					.Include(m => m.Chats)
					.Where(a => accounts.Select(m => m.Id).Contains(a.AccountId))
					.ToListAsync(cancellationToken);


				if (dbAccounts == null)
				{
					Log.Error($"{nameof(GPM5903)} dbAccounts not found");
					return;
				}

				Log.Information($"{nameof(GPM5903)} dbAccounts  {dbAccounts?.Count() ?? 0} {string.Join(',', dbAccounts.Select(c => c.AccountId.ToString()))}");


				foreach (var item in dbAccounts)
				{
					chats.Where(c => !item.Chats.Select(m => m.ChatId).Contains(c.ChatId))
						.ToList()
						.ForEach(c => {
							item.Chats.Add(new Domains.Entities.AccountChat()
							{
								ChatId = c.ChatId,
								AccountId = item.AccountId
							});

							Log.Information($"{nameof(GPM5903)} Account {item.Login} added to chat {c.ChatId} {c.Name}");

						});

					_context.Accounts.Update(item);
				}

				_context.SaveChanges();
			}

			Log.Information($"{nameof(GPM5903)} complited");
		}
	}
}
