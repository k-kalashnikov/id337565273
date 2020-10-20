using MediatR;
using SP.Consumers.Models;

namespace SP.Messenger.Application.Accounts.Queries.GetAccountFromIdentityServer
{
    public class GetAccountFromIdentityServerEmailQuery : IRequest<ApplicationUser>
    {
        public string Email { get; }

        public GetAccountFromIdentityServerEmailQuery(string email)
        {
            Email = email;
        }
        
        public  static  GetAccountFromIdentityServerEmailQuery Create(string email)
        => new GetAccountFromIdentityServerEmailQuery(email);
    }
}