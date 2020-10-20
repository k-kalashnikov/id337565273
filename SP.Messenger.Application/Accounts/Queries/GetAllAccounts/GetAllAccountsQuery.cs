using MediatR;
using SP.Consumers.Models;

namespace SP.Messenger.Application.Accounts.Queries.GetAllAccounts
{
    public class GetAllAccountsQuery : IRequest<AccountMessengerDTO[]>
    {
    }
}
