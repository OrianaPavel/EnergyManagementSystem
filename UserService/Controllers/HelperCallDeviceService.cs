using System.Configuration;
using System.Net.Http;
using HashidsNet;

namespace UserService.Controllers
{
    public class HelperCallDeviceService
    {   
        private readonly ILogger<HelperCallDeviceService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IHashids _hashids;

        public HelperCallDeviceService( ILogger<HelperCallDeviceService> logger,HttpClient httpClient, IHashids hashids){
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _hashids = hashids ?? throw new ArgumentNullException(nameof(hashids));
        }


        public void CreateUserInDevice(string userId, String? authorizationToken)
        {
            _logger.LogError($"Error creating user in DeviceUser API: auth token is ----> " + authorizationToken);
            if (!string.IsNullOrEmpty(authorizationToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authorizationToken);
            }
         
            var response = _httpClient.PostAsync($"User/{userId}", null).Result;

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error creating user in DeviceUser API: {response.StatusCode} + ---- > {userId}");
            }
        }

        public void DeleteUser(string userId, String? authorizationToken)
        {
            if (!string.IsNullOrEmpty(authorizationToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authorizationToken);
            }

            var response = _httpClient.DeleteAsync($"user/{userId}").Result;

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error creating user in DeviceUser API: {response.StatusCode} + ---- > {userId}");
            }
        }

    }
    
}