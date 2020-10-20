using System.Collections.Generic;
using System.Threading.Tasks;

namespace SP.Messenger.Hub.Service.Services
{
    public interface IActiveUsersService
    {
        Task AddUser(string connectionId, string email);
        Task RemoveUser(string email, string connectionId);
        Task<bool> IsActive(string email);
        Task<ActiveUser> GetUsersByEmail(string email);
        Task<IEnumerable<ActiveUser>> GetUsersByEmail(IEnumerable<string> emails);
        Task<ActiveUser> GetUsersByConnectionId(string connectionId);
    }
}
