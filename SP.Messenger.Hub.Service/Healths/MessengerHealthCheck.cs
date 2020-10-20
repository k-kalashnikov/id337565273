using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SP.Messenger.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace SP.Messenger.Hub.Service.Healths
{
    public class MessengerHealthCheck : IHealthCheck
    {
        public static string Name => "Healthy";
        private readonly MessengerDbContext _context;

        public MessengerHealthCheck(MessengerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var documentTypes = await _context.DocumentTypes.AsNoTracking().ToListAsync(cancellationToken);
                documentTypes = null;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                
                return HealthCheckResult.Unhealthy("error");
            }
            return HealthCheckResult.Healthy();
        }
    }
}
