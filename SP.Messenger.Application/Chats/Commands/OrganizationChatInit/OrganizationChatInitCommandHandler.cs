using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SP.Consumers.Models;
using SP.FileStorage.Client.Services;
using SP.Market.Core.Messages.Bulders;
using SP.Market.EventBus.RMQ.Shared.Events.Users;
using SP.Messenger.Application.Accounts.Commands;
using SP.Messenger.Application.Chats.Commands.CreateChat;
using SP.Messenger.Common.Contracts;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Chats.Commands.OrganizationChatInit
{
    public class OrganizationChatInitCommandHandler : IRequestHandler<OrganizationChatInitCommand, ProcessingResult<bool>>
    {
        private readonly MessengerDbContext Context;
        private readonly IMediator Mediator;
        private readonly IFileStorageClientService FileStorage;

        public OrganizationChatInitCommandHandler(MessengerDbContext _context, IMediator _mediator, IFileStorageClientService _fileStorage)
        {
            Context = _context ?? throw new ArgumentNullException(nameof(_context));
            Mediator = _mediator ?? throw new ArgumentNullException(nameof(_mediator));
            FileStorage = _fileStorage ?? throw new ArgumentNullException(nameof(_fileStorage));
        }

        public async Task<ProcessingResult<bool>> Handle(OrganizationChatInitCommand _request, CancellationToken _cancellationToken)
        {
            try
            {
                var currentAccounts = await Context.Accounts.ToListAsync();
                var currentChats = await Context.ChatView.ToListAsync();

                var newAccounts = _request.Accounts.Where(m => !currentAccounts.Any(a => a.Login.Equals(m.Login)));
                var newChatsView = _request.Organizations.Where(m => !currentChats.Any(c => c.DocumentId.Equals(m.OrganizationGUID.ToString())));

                foreach (var item in newChatsView)
                {
                    var tempChatAccounts = newAccounts
                        .Where(m => m.OrganizationGuid.Equals(item.OrganizationGUID))
                        .Select(m => new AccountMessengerDTO() {
                            AccountId = m.AccountId,
                            FirstName = m.FirstName,
                            LastName = m.LastName,
                            Login = m.Login,
                            MiddleName = m.MiddleName
                        }).ToArray();

                    var createChatCommand = CreateChatCommand.Create(
                            item.Name,
                            "module.mdm.chat.organization.chat.public",
                            true,
                            item.OrganizationGUID,
                            7,
                            string.Empty,
                            SP.Consumers.Models.ModuleName.Market,
                            tempChatAccounts
                        );

                    var resultCommand = await Mediator.Send(createChatCommand, _cancellationToken);
                }

                return new ProcessingResult<bool>(true);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                Console.WriteLine($"Don`t write chat name: {e}");
                return new ProcessingResult<bool>(false, new IError[] { new SimpleResponseError(e.Message), });

            }

        }
    }
}
