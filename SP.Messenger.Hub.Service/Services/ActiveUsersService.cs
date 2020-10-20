using SP.Messenger.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace SP.Messenger.Hub.Service.Services
{
    public class ActiveUsersService : IActiveUsersService
    {
        private readonly ICacheService _cache;

        public ActiveUsersService(ICacheService cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task AddUser(string connectionId, string login)
        {
            //try
           // {
                var user = await _cache.GetAsync<ActiveUser>(login, CancellationToken.None);
                if (user is null)
                {
                    user = new ActiveUser(login);
                    user.AddConnection(connectionId);
                    
                    await _cache.SetAsync(login, user, 60, 150, CancellationToken.None);
                }
                else
                {
                    user.AddConnection(connectionId);
                    await _cache.SetAsync(login, user, 60, 150, CancellationToken.None);
                }
            /*
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(ActiveUsersService)} - {nameof(AddUser)}: {ex}");
            }
            */
        }

        public async Task<bool> IsActive(string login)
        {
            /*
            try
            {
            */
                var user = await _cache.GetAsync<ActiveUser>(login, CancellationToken.None);
                return user is null;

            /*
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(ActiveUsersService)} - {nameof(IsActive)}: {ex}");
            }
            */
            return false;
        }

        public async Task<ActiveUser> GetUsersByEmail(string email)
        {
            return await _cache.GetAsync<ActiveUser>(email, CancellationToken.None);
        }

        public async Task<IEnumerable<ActiveUser>> GetUsersByEmail(IEnumerable<string> emails)
        {
            var activeUsers = new List<ActiveUser>();
            foreach (var email in emails)
            {
                if(string.IsNullOrEmpty(email))
                    continue;
                
                var user = await _cache.GetAsync<ActiveUser>(email, CancellationToken.None);
                if (user != null)
                {
                    activeUsers.Add(user);
                }
            }

            return activeUsers;
        }

        public async Task RemoveUser(string email, string connectionId)
        {
            /*
            try
            {
            */
                var user = await _cache.GetAsync<ActiveUser>(email, CancellationToken.None);

                if (user != null)
                {
                    user.DeleteConnection(connectionId);
                    await _cache.SetAsync(email, user, 60, 150, CancellationToken.None);    
                }
            /*
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(ActiveUsersService)} - {nameof(RemoveUser)}: {ex}");
            }
            */
        }

        public async Task<ActiveUser> GetUsersByConnectionId(string connectionId)
        {
            var user = await _cache.GetAsync<ActiveUser>(connectionId, CancellationToken.None);
            return user;
        }
    }



    public class ActiveUser 
    {
        private ActiveUser()
        {
            ConnectionIds = new List<string>();
        }

        public ActiveUser(string login)
            :this()
        {
            Login = login;
        }

        public IList<string> ConnectionIds { get; }
        public string Login { get; }

        public void AddConnection(string connectionId)
        {
            if (ConnectionIds.FirstOrDefault(x => x == connectionId) == null)
            {
                ConnectionIds.Add(connectionId);
            }
        }
        public void DeleteConnection(string connectionId)
        {
            if (ConnectionIds.FirstOrDefault(x => x == connectionId) != null)
            {
                ConnectionIds.Remove(connectionId);
            }
        }
    }
}
