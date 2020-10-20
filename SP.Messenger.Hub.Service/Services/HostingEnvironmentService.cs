using SP.Market.Core.Hosting;

namespace SP.Messenger.Hub.Service.Services
{
    public class HostingEnvironmentService: IHostingEnvironmentService
    {
        private bool isProduction;

        public bool GetEnvironment() => isProduction;

        public void SetEnvironment(bool isProduction)
        {
            this.isProduction = isProduction;
        }
    }
}