using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SP.Messenger.Hub.Service.AssemblyMigrations;

namespace SP.Messenger.Hub.Service.Services
{
    public class AssemblyMigrationService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly Dictionary<string, List<Type>> _tasks = new Dictionary<string, List<Type>>(){
            { 
                "1.0.0.0", 
                new List<Type>()
                {
					// typeof(GPM6557),
					// typeof(GPM5214),
					// typeof(GPM5635),
					// typeof(GPM5903),
					// typeof(GPM5904),
					// typeof(GPM5905),
					//typeof(GPM6210) - TODO - узнать у менеджеров
                    typeof(CreateBucketsMigration),
                    typeof(CreateChatContract)
                }
            }
        };

        public AssemblyMigrationService(IServiceProvider serviceProvider) 
		{
            _serviceProvider = serviceProvider;
        }

        private Task DoWork(CancellationToken cancellationToken) 
        {
            var version = GetType().Assembly.GetName().Version.ToString();
            var tasks = _tasks[version];
			foreach (var item in tasks)
			{
                try
                {
                    var tempTask = (IAssemblyMigration)Activator.CreateInstance(item, _serviceProvider);
                    tempTask.DoWork(cancellationToken);
                }
                catch (Exception e) 
                {
                    Log.Error(e.Message);
                }
			}

            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
           return DoWork(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
	}
}
