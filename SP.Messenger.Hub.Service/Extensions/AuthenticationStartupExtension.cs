using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using SP.Market.Identity.Common.Services;

namespace SP.Messenger.Hub.Service.Extensions
{
    public static class AuthenticationStartupExtension
    {

        public static IServiceCollection AddAuthenticationService(this IServiceCollection services,
            IConfiguration configuration)
        {

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            //LifetimeValidator = (before, expires, token, param) =>
                            //{
                            //    return expires > DateTime.UtcNow;
                            //},
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = AuthorizationOptions.ISSUER,
                            ValidAudience = AuthorizationOptions.AUDIENCE,
                            IssuerSigningKey = AuthorizationOptions.Create(AuthorizationOptions.KEY)
                        };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            string accessToken;
                            KeyValuePair<string, StringValues>? header;

                            header = context.Request.Headers.FirstOrDefault(x => x.Key == "Authorization");
                            accessToken = header.Value.Value;
                            if (string.IsNullOrEmpty(accessToken))
                            {
                                var token = context.Request.Query["access_token"];
                                if (!string.IsNullOrEmpty(token))
                                    accessToken = token;
                            }

                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/messenger/negotiate")
                                 || path.StartsWithSegments("/messenger")))
                                context.Token = accessToken.Replace("Bearer ", string.Empty);

                            return Task.CompletedTask;
                        }
                    };
                });
            return services;
        }

    }
}
