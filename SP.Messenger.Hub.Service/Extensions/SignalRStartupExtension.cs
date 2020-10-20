using System;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace SP.Messenger.Hub.Service.Extensions
{
    public static class SignalRStartupExtension
    {
        public static IServiceCollection AddSignalRService(this IServiceCollection services, IConfiguration config)
        {
            services.AddSignalR(opt => { opt.EnableDetailedErrors = true; })
                .AddNewtonsoftJsonProtocol();
                /*
                .AddStackExchangeRedis(c =>
                {
                    c.ConnectionFactory = async w =>
                    {
                        var options = new ConfigurationOptions
                        {
                            AbortOnConnectFail = false,
                            DefaultDatabase = int.Parse(config["ConnectionRedisClient:DefaultDatabase"])
                        };

                        options.EndPoints.Add(IPAddress.Parse(config["ConnectionRedisClient:Host"]),
                            int.Parse(config["ConnectionRedisClient:Port"]));

                        options.Password = config["ConnectionRedisClient:Password"];
                        var connection = await ConnectionMultiplexer.ConnectAsync(options, w);
                        connection.ConnectionFailed += (o, e) =>
                        {
                            Console.WriteLine("SignalR connection to Redis failed.");
                        };

                        Console.WriteLine(connection.IsConnected
                            ? "SignalR connected to Redis success."
                            : "SignalR did not connect to Redis.");

                        return connection;
                    };
                });
                */
            
            return services;
        }
    }
}
