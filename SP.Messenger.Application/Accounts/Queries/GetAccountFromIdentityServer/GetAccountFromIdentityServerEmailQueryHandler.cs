using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SP.Consumers.Models;
using SP.Messenger.Infrastructure.Services;

namespace SP.Messenger.Application.Accounts.Queries.GetAccountFromIdentityServer
{
    public class GetAccountFromIdentityServerEmailQueryHandler : 
        IRequestHandler<GetAccountFromIdentityServerEmailQuery, ApplicationUser>
    {
        private readonly IAccountIdentityService<GetAccountIdentityRequest, GetAccountIdentityResponse> _accountService; 
        public GetAccountFromIdentityServerEmailQueryHandler(
            IAccountIdentityService<GetAccountIdentityRequest, GetAccountIdentityResponse> accountService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        }


        public async Task<ApplicationUser> Handle(GetAccountFromIdentityServerEmailQuery request, CancellationToken cancellationToken)
        {
            var accountRequest = GetAccountIdentityRequest.Create(request.Email);
            var response = await _accountService.GetResponseAsync(accountRequest, cancellationToken);
            return ApplicationUser.Create(response);
        }
    }
}