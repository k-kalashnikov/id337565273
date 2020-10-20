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
	public class GPM5635 : IAssemblyMigration
	{
		public IServiceProvider ServiceProvider { get; set; }

		private MessengerDbContext _context;


		public GPM5635(IServiceProvider serviceProvider)
		{
			ServiceProvider = serviceProvider;
		}

		public async Task DoWork(CancellationToken cancellationToken)
		{
			Log.Information($"{nameof(GPM5635)} invoked");

			using (var scope = ServiceProvider.CreateScope())
			{
				_context = scope.ServiceProvider.GetRequiredService<MessengerDbContext>();

				var chat = await _context.ChatsParentsView
					.Include(m => m.Chat)
					.FirstOrDefaultAsync(m => m.DocumentId.Equals("07e1136b-74d6-427c-85b2-2791505a8334"));

				if (chat == null)
				{
					Log.Error($"{nameof(GPM5635)} chat not found");
					return;
				}

				Log.Information($"{nameof(GPM5635)} {chat.ChatId} chat found");


				var accountChats = await _context.AccountChatView
					.Where(ac => ac.ChatId.Equals(chat.ChatId))
					.ToListAsync();

				if (accountChats == null)
				{
					Log.Error($"{nameof(GPM5635)} accountChats not found");
					return;
				}


				Log.Information($"{nameof(GPM5635)} {accountChats.Count()} accountChats found");

				var neededAccounts = await _context.Accounts
					.Where(a => !accountChats.Select(ac => ac.AccountId).ToList().Contains(a.AccountId))
					.Include(m => m.Chats)
					.ToListAsync();


				if (neededAccounts == null)
				{
					Log.Error($"{nameof(GPM5635)} accounts not found");
					return;
				}

				Log.Information($"{nameof(GPM5635)} {neededAccounts.Count()} neededAccounts found");


				foreach (var item in neededAccounts)
				{
					item.Chats.Add(new Domains.Entities.AccountChat() {
						ChatId = chat.ChatId,
						AccountId = item.AccountId
					});

					Log.Information($"{nameof(GPM5635)} {item.Login} added to {chat.Chat.Name} chat");

					_context.Accounts.Update(item);
				}

				_context.SaveChanges();
			}
			Log.Information($"{nameof(GPM5635)} complited");
		}
	}
}
