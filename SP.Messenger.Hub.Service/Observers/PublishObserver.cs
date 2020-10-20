using MassTransit;
using Microsoft.AspNetCore.Http;
using Serilog;
using SP.Market.Identity.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SP.Messenger.Hub.Service.Observers
{
    public class PublishObserver : IPublishObserver
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICurrentUserService _currentUserService;
        public PublishObserver(ICurrentUserService currentUserService, IHttpContextAccessor contextAccessor)
        {
            _currentUserService = currentUserService;
            _contextAccessor = contextAccessor;
        }

        public Task PostPublish<T>(PublishContext<T> context) where T : class
        {
            return Task.CompletedTask;
        }

        public Task PrePublish<T>(PublishContext<T> context) where T : class
        {
            var jwt = _currentUserService.GetCurrentUser()?.SecurityToken;

            if (string.IsNullOrEmpty(jwt?.ToString())) 
            {
                jwt = _contextAccessor.HttpContext?.Request.Query["access_token"];
            }

            context.Headers.Set("Authorization", jwt);

            Log.Information($"**** {nameof(PublishObserver)} {nameof(PrePublish)} JWT: {jwt}");
            return Task.CompletedTask;
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
        {
            return Task.CompletedTask;
        }
    }
}
