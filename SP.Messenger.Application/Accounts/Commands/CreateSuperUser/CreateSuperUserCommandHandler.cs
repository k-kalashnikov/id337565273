
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SP.Messenger.Application.Accounts.Builders;
using SP.Messenger.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Accounts.Commands.CreateSuperUser
{
	public class CreateSuperUserCommandHandler : IRequestHandler<CreateSuperUserCommand, long>
	{
		private readonly MessengerDbContext _context;

		public CreateSuperUserCommandHandler(MessengerDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<long> Handle(CreateSuperUserCommand request, CancellationToken cancellationToken)
		{
			Log.Information($"{nameof(CreateSuperUserCommandHandler)} invoked");

			var accountDb = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == request.AccountId, cancellationToken);

			if (accountDb == null) 
			{
				var account = Create.Account(request.AccountId, request.Login, request.FirstName, request.LastName, request.MiddleName).Build();

				await _context.Accounts.AddAsync(account, cancellationToken);

				await _context.SaveChangesAsync(cancellationToken);

			}

			var chats = await _context.Chats.ToListAsync(cancellationToken);

			var newSuperUser = await _context.Accounts
				.Include(m => m.Chats)
				.FirstOrDefaultAsync(a => a.AccountId.Equals(request.AccountId));

			Log.Information($"{nameof(CreateSuperUserCommandHandler)} Found {chats.Count()} chats");

			var neededChat = chats.Where(c => !newSuperUser.Chats.Select(m => m.ChatId).Contains(c.ChatId)).ToList();

			Log.Information($"{nameof(CreateSuperUserCommandHandler)} Found neededChat {neededChat.Count()} chats");

			neededChat.ForEach(c => {
				newSuperUser.Chats.Add(new Domains.Entities.AccountChat()
				{
					ChatId = c.ChatId,
					AccountId = newSuperUser.AccountId
				});

				Log.Information($"{nameof(CreateSuperUserCommandHandler)} Account {newSuperUser.Login} added to chat {c.ChatId} {c.Name}");
			});

			_context.Accounts.Update(newSuperUser);

			await _context.SaveChangesAsync(cancellationToken);

			Log.Information($"{nameof(CreateSuperUserCommandHandler)} complited");

			return newSuperUser.AccountId;
		}
	}
}
