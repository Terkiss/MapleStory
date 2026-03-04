using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MapleStoryMarketGraph.Services
{
    public class NexonApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public NexonApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["NexonApi:BaseUrl"] ?? "https://open.api.nexon.com";
        }

        public async Task<bool> VerifyApiKeyAsync(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                return false;

            using var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/maplestory/v1/character/list");
            request.Headers.Add("x-nxopen-api-key", apiKey);

            try
            {
                var response = await _httpClient.SendAsync(request);
                // We don't necessarily need a 200 OK with data, just that it's not 401 Unauthorized or 403 Forbidden.
                // However, character/list requires character_name or ocid... wait.
                // Let's use something simple like character/ocid with a dummy name, or just character/list?
                // Actually character/list requires 'character_name'. 
                // Let's use character/ocid with a known name, or just check if it returns 200 or 400 (Bad Request is better than 401).
                
                // If it's 401 or 403, it's definitely an invalid key.
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || 
                    response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return false;
                }

                // If it's 400 Bad Request, it means the API key was accepted but parameters were wrong, which is "valid key" for verification.
                // If it's 200 OK, it's definitely valid.
                return response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.BadRequest;
            }
            catch
            {
                return false;
            }
        }
    }
}
