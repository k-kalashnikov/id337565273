using System;
using System.Net.Http;
using System.Threading.Tasks;
using SP.Market.Core.Tokens;


namespace SP.Messenger.Hub.Service.Services
{
    public class CacheManager
    {
        private readonly HttpClient _client;

        public CacheManager(HttpClient client)
            => (_client) = (client);
        
        public virtual async Task<T> GetSessionUser<T>(string api, string token)
        {
            if (string.IsNullOrEmpty(api) || string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(api));

            T sessionUser;

            try
            {
                var response = await _client.GetAsync($"{api}{token}");
                sessionUser = await response.Content.ReadAsAsync<T>();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.ToString());
            }

            return sessionUser;
        }
        
        #region Session
        public async Task<bool> SaveSessionUser<T>(T session)
        {
            var response = await _client.PostAsJsonAsync("api/v1/tokencache", session);
            if (response.IsSuccessStatusCode)
                return true;
            else
                return false;
        }

        public async Task<bool> RemoveSessionUser(string key)
        {
            var response = await _client.DeleteAsync($"api/v1/tokencache?key={key}");
            if (response.IsSuccessStatusCode)
                return true;
            else
                return false;
        }

        public async Task<SessionUser> GetSessionByToken(string token)
        {
            var response = await _client.GetAsync($"api/v1/tokencache?key={token}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsAsync<SessionUser>();
            else
                return null;
        }
        #endregion
    }
}