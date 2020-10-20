using System;
using System.Linq;
using System.Reflection;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SP.Accounts.Client;
using SP.Contract.Client;
using SP.FileStorage.Client;
using SP.Market.Identity.Client;
using SP.Market.Identity.Common.Interfaces;
using SP.Market.Identity.Common.Services;
using SP.Messenger.Application.Infrastructure;
using SP.Messenger.Application.Interfaces;
using SP.Messenger.Application.Messages.Command;
using SP.Messenger.Client;
using SP.Messenger.Common.Interfaces;
using SP.Messenger.Common.Settings;
using SP.Messenger.Hub.Service.Extensions;
using SP.Messenger.Hub.Service.Filters;
using SP.Messenger.Hub.Service.Hub;
using SP.Messenger.Hub.Service.Middleware;
using SP.Messenger.Hub.Service.Services;
using SP.Messenger.Infrastructure.Services;
using SP.Messenger.Infrastructure.Services.Accounts;
using SP.Messenger.Infrastructure.Services.Bids;
using SP.Messenger.Infrastructure.Services.Cache;
using SP.Messenger.Infrastructure.Services.Report;
using SP.Messenger.Persistence;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace SP.Messenger.Hub.Service
{
    public class Startup
    {
        private const string MessengerClientQueue = "messenger.client";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(Configuration)
                .AddMassTransitService(Configuration)
                .AddAppHealthChecks(Configuration);


            services.AddDbContext<MessengerDbContext>(options =>
            {
                options.EnableDetailedErrors();
#if DEBUG
                options.UseLoggerFactory(GetDbLoggerFactory()).EnableSensitiveDataLogging();
#endif
                options.UseNpgsql(Configuration.GetSection("ConnectionStrings:PostgresConnection:connectionString")
                                      .Value +
                                  $"Database={Configuration.GetSection("ConnectionStrings:PostgresConnection:Database").Value};");
            });

            var keys = Configuration.AsEnumerable().Where(x => x.Key.Contains("Cors")).ToArray();
            var corsParams = new string[keys.Length];
            for (var i = 0; i < keys.Length; i++)
                corsParams[i] = keys[i].Value;

            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder.WithOrigins(corsParams.Where(x => x != null).ToArray())
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }));

            services.AddOptions();
            services.Configure<Settings>(Configuration);

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddControllers(options => { options.Filters.Add(new CustomExceptionFilterAttribute()); })
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddHttpContextAccessor();
            services.AddTransient<ICurrentUserService, CurrentUserService>();
            services.AddTransient<IHostingEnvironmentService, HostingEnvironmentService>();
            services.AddTransient(typeof(IAccountPlatformService<,>), typeof(AccountPlatformService<,>));
            services.AddScoped(typeof(IAccountIdentityService<,>), typeof(AccountIdentityService<,>));
            services.AddScoped(typeof(IGetBidStatusService<,>), typeof(GetBidStatusService<,>));
            services.AddScoped(typeof(IContragentsProject<,>), typeof(ContragentsProject<,>));
            services.AddScoped(typeof(IAccountsOrganizationService<,>), typeof(AccountsOrganizationService<,>));
            services.AddTransient(typeof(IMessengerRequestClientService<,>), typeof(MessengerRequestClientService<,>));

            services.AddTransient<IActiveUsersService, ActiveUsersService>();

            services.AddTransient<Mediator>();

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            services.AddMediatR(typeof(SaveMessageCommandHandler).GetTypeInfo().Assembly);


            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ApiBehaviorInvalidModelResponse.Response;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "SP.Messenger.Hub.Service"
                });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.CustomSchemaIds(x => x.FullName);
            });

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddAuthenticationService(Configuration);

            services.AddStackExchangeRedisCache(opt =>
            {
                opt.Configuration =
                    $"{Configuration["ConnectionRedisClient:Host"]}:{Configuration["ConnectionRedisClient:Port"]}," +
                    $"password={Configuration["ConnectionRedisClient:Password"]}," +
                    $"DefaultDatabase={Configuration["ConnectionRedisClient:CacheDatabase"]}";
            });

            services.AddSignalRService(Configuration);

            services.AddScoped<ICacheService, CacheService>();

            services.AddScoped(typeof(IAccountIdentityService<,>), typeof(AccountIdentityService<,>));

            services.AddMarketIdentity(new AuthenticationClientOptions
            {
                Uri = new UriBuilder($"{Configuration.GetSection("Services:IdentityService").Value}"),
                IsIncludeOneFactorAuthotication = false
            });

            services.AddScoped(typeof(IGetBidStatusService<,>), typeof(GetBidStatusService<,>));

            services
                .AddFileStorageClient(opt => { opt.Uri = Configuration["Services:FileStorageService"]; })
                .AddContractClient(opt => opt.Uri = Configuration["Services:ContractService"])
                .AddAccountsClient(opt => opt.Uri = Configuration["Services:AccountsService"]);


            services.AddHostedService<AssemblyMigrationService>();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseRouting();
            app.UseSwagger();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => true
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./v1/swagger.json",
                    "swagger by SP.Messenger.Hub.Service V1");
                //c.RoutePrefix = string.Empty;
            });

            app.UseCors("CorsPolicy");

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<MessengerHub>("/messenger");
                endpoints.MapControllers();
            });

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            InitializerDatabase(app);
            DefineEnvironment(app, env);
        }

        private static void InitializerDatabase(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetService<MessengerDbContext>();
            MessengerInitializer.InitializeAsync(context);
        }
        private static void DefineEnvironment(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var environmentService = app.ApplicationServices.GetRequiredService<IHostingEnvironmentService>();
            environmentService.SetEnvironment(env.IsProduction());
        }

        private static ILoggerFactory GetDbLoggerFactory()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
                builder.AddDebug()
                    .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information));
            return serviceCollection.BuildServiceProvider().GetService<ILoggerFactory>();
        }
    }
}