namespace SP.Messenger.Hub.Service.Services
{
    public interface IHostingEnvironmentService
    {
        void SetEnvironment(bool isProduction);
        bool GetEnvironment();
    }
}