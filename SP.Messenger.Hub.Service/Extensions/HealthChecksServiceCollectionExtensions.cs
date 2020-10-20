using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using RabbitMQ.Client;

namespace SP.Messenger.Hub.Service.Extensions
{
    public static class HealthChecksServiceCollectionExtensions
    {
        public static IServiceCollection AddAppHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureHttps(services);

            services.AddHealthChecks()
                .AddRabbitMQ(
                    $"amqp://{configuration["RMQClient:UserName"]}:{configuration["RMQClient:Password"]}@" +
                    $"{configuration["RMQClient:Host"]}:5672",
                    sslOption: new SslOption(serverName: configuration["RMQClient:Host"], enabled: false),
                    name: "RabbitMQ")
                .AddElasticsearch(configuration["Serilog:Elasticsearch"], name: "ElasticSearch")
                .AddNpgSql(npgsqlConnectionString: configuration["ConnectionStrings:PostgresConnection:connectionString"], name: "PostgreSQL")
                .AddUrlGroup(
                    new Uri(
                        new Uri(configuration["Services:IdentityService"]).GetLeftPart(System.UriPartial.Authority) +
                        "/hc"), name: "IdentityService");
            /*.AddUrlGroup(new Uri(new Uri(configuration["MetricsReportingInfluxDbOptions:InfluxDb:BaseUri"]).GetLeftPart(System.UriPartial.Authority) + "/ping"), name: "InfluxDb");*/

            return services;
        }

        private static void ConfigureHttps(IServiceCollection services)
        {
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .RetryAsync(5);

            services.AddHttpClient("IdentityService")
                .AddPolicyHandler(retryPolicy)
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
                });
        }
    }
}
