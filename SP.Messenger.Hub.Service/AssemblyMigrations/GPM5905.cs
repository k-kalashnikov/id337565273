using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SP.Messenger.Application.Accounts.Commands.CreateSuperUser;
using SP.Messenger.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Hub.Service.AssemblyMigrations
{
	public class GPM5905 : IAssemblyMigration
	{
		public IServiceProvider ServiceProvider { get; set; }
		private MessengerDbContext _context;
		private IMediator _mediator;


		private readonly List<CreateSuperUserCommand> _accounts = new List<CreateSuperUserCommand>() {
			new CreateSuperUserCommand()
			{
				AccountId = 23960,
				FirstName = "Anton",
				LastName = "Zhurihin",
				Login = "anton.zhurihin@stecpoint.ru",
				MiddleName = "blabla"
			},
			new CreateSuperUserCommand()
			{
				AccountId = 26961,
				FirstName = "pavel",
				LastName = "belov",
				Login = "pavel.belov@stecpoint.ru",
				MiddleName = "blabla"
			},
			new CreateSuperUserCommand()
			{
				AccountId = 26962,
				FirstName = "natalia",
				LastName = "popykina",
				Login = "natalia.popykina@stecpoint.ru",
				MiddleName = "blabla"
			},
			new CreateSuperUserCommand()
			{
				AccountId = 22262,
				FirstName = "nickolay",
				LastName = "zlobin",
				Login = "nickolay.zlobin@stecpoint.ru",
				MiddleName = "blabla"
			},
			new CreateSuperUserCommand()
			{
				AccountId = 22250,
				FirstName = "anton",
				LastName = "iliin",
				Login = "valroy.test5@inbox.ru",
				MiddleName = "blabla"
			}
		};

		public GPM5905(IServiceProvider serviceProvider)
		{
			ServiceProvider = serviceProvider;
		}

		public async Task DoWork(CancellationToken cancellationToken)
		{
			Log.Information($"{nameof(GPM5905)} invoked");
			using (var scope = ServiceProvider.CreateScope())
			{
				_context = scope.ServiceProvider.GetRequiredService<MessengerDbContext>();
				_mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

				foreach (var item in _accounts)
				{
					Log.Information($"{nameof(GPM5905)} invoke CreateSuperUserCommand for user {item.Login}");
					await _mediator.Send(item, cancellationToken);
				}
			}

			Log.Information($"{nameof(GPM5905)} complited");

		}
	}
}
