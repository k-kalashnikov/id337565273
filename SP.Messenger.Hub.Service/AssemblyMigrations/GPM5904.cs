using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SP.Messenger.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Hub.Service.AssemblyMigrations
{
	public class GPM5904 : IAssemblyMigration
	{
		public IServiceProvider ServiceProvider { get; set; }
		private MessengerDbContext _context;

		private readonly List<string> _accounts = new List<string>() {
			"popovana@roslep.com",
			"comdir@roslep.com"
		};

		public GPM5904(IServiceProvider serviceProvider)
		{
			ServiceProvider = serviceProvider;
		}

		public async Task DoWork(CancellationToken cancellationToken)
		{
			Log.Information($"{nameof(GPM5904)} invoked");
			using (var scope = ServiceProvider.CreateScope())
			{
				_context = scope.ServiceProvider.GetRequiredService<MessengerDbContext>();

				var chats = await _context.Chats
					.Where(m => m.Name.Contains("СК002004419_Блок4_НПЗ"))
					.ToListAsync(cancellationToken);

				if (chats == null)
				{
					Log.Error($"{nameof(GPM5904)} Chats not found");
					return;
				}

				Log.Information($"{nameof(GPM5904)} Found chats {chats?.Count() ?? 0} {string.Join(',', chats.Select(c => c.ChatId.ToString()))}");


				var accounts = await _context.Accounts
					.Include(m => m.Chats)
					.Where(a => _accounts.Contains(a.Login))
					.ToListAsync();

				if (accounts == null)
				{
					Log.Error($"{nameof(GPM5904)} accounts not found");
					return;
				}

				Log.Information($"{nameof(GPM5904)} Found accounts {accounts?.Count() ?? 0} {string.Join(',', accounts.Select(c => c.AccountId.ToString()))}");

				foreach (var item in accounts)
				{
					chats.Where(c => !item.Chats.Select(m => m.ChatId).Contains(c.ChatId))
						.ToList()
						.ForEach(c => {
							item.Chats.Add(new Domains.Entities.AccountChat()
							{
								ChatId = c.ChatId,
								AccountId = item.AccountId
							});

							Log.Information($"{nameof(GPM5904)} Account {item.Login} added to chat {c.ChatId} {c.Name}");

						});

					_context.Accounts.Update(item);
				}

				_context.SaveChanges();
			}

			Log.Information($"{nameof(GPM5904)} complited");

		}
	}
}
