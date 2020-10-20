using MassTransit;
using Microsoft.AspNetCore.Http;
using Serilog;
using SP.Market.Core.Helpers;
using SP.Market.Identity.Common.Interfaces;
using System;
using System.Threading.Tasks;

namespace SP.Messenger.Hub.Service.Observers
{
    public class ReceiveObserver : IReceiveObserver
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReceiveObserver(ICurrentUserService currentUserService, IHttpContextAccessor httpContextAccessor)
        {
            _currentUserService = currentUserService;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception) where T : class
        {
            return Task.CompletedTask;
        }

        public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
        {
            return Task.CompletedTask;
        }

        public Task PostReceive(ReceiveContext context)
        {
            return Task.CompletedTask;
        }

        public Task PreReceive(ReceiveContext context)
        {
            var jwt = context.TransportHeaders.Get("Authorization", string.Empty);

            var currentUser = _currentUserService.CreateUserByToken(jwt);

            _httpContextAccessor.HttpContext = HttpContextHelper.BuildContext(currentUser.Id, currentUser.Login, jwt);

            Log.Information($"**** {nameof(ReceiveObserver)} {nameof(PreReceive)} JWT: {jwt}");

            return Task.CompletedTask;
        }

        public Task ReceiveFault(ReceiveContext context, Exception exception)
        {
            return Task.CompletedTask;
        }
    }
}
