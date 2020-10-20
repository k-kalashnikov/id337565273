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
	public class GPM5214 : IAssemblyMigration
	{
		public IServiceProvider ServiceProvider { get; set; }
		private readonly List<string> _needChats = new List<string>()
		{
			"СК002003414_Блок4_НПЗ",
			"СК002001231",
			"СК002001102"
		};

		private MessengerDbContext _context;

		public GPM5214(IServiceProvider serviceProvider)
		{
			ServiceProvider = serviceProvider;
		}

		public async Task DoWork(CancellationToken cancellationToken)
		{
			Log.Information($"{nameof(GPM5214)} invoked");

			using (var scope = ServiceProvider.CreateScope())
			{
				_context = scope.ServiceProvider.GetRequiredService<MessengerDbContext>();

				var user = await _context.Accounts
					.Include(m => m.Chats)
					.FirstOrDefaultAsync(m => m.Login.Equals("shushpannikovatn@gsp-k.ru"));

				if (user == null)
				{
					Log.Error($"{nameof(GPM5214)} user not found");
					return;
				}

				var chats = await _context.Chats.Where(m => _needChats.Contains(m.Name))?.ToListAsync();
				if (chats == null) 
				{
					Log.Error($"{nameof(GPM5214)} chats not found");
					return;
				}

				Log.Information($"{nameof(GPM5214)} {string.Join(',', chats.Select(m => m.Name))} chats founded");


				var userChars = await _context.AccountChatView
					.Where(uc => chats.Any(c => c.ChatId.Equals(uc.ChatId)))
					.Where(uc => uc.AccountId.Equals(user.AccountId))
					.ToListAsync();

				foreach (var item in chats.Where(c => !userChars.Any(uc => uc.ChatId.Equals(c.ChatId))))
				{
					user.Chats.Add(new Domains.Entities.AccountChat()
					{
						AccountId = user.AccountId,
						ChatId = item.ChatId
					});

				}
				_context.Accounts.Update(user);
				_context.SaveChanges();
			}

			Log.Information($"{nameof(GPM5214)} complited");
		}
	}
}
