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

            // Using /maplestory/v1/id as a simple verification endpoint. 
            // It needs a character_name, but if we get a 200 or 400 (not 401/403), the key is valid.
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/maplestory/v1/id?character_name=test");
            request.Headers.Add("x-nxopen-api-key", apiKey);

            try
            {
                var response = await _httpClient.SendAsync(request);
                return response.StatusCode != System.Net.HttpStatusCode.Unauthorized &&
                       response.StatusCode != System.Net.HttpStatusCode.Forbidden;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string?> GetRepresentativeCharacterNameAsync(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey)) return null;

            // In a real scenario, we'd fetch the character list and pick one.
            // For now, let's assume we can fetch the list.
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/maplestory/v1/character/list");
            request.Headers.Add("x-nxopen-api-key", apiKey);

            try
            {
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    // Logic to parse JSON and return the first character name
                    // var data = await response.Content.ReadFromJsonAsync<CharacterListResponse>();
                    // return data?.AccountCharacters?.FirstOrDefault()?.CharacterName;
                    return "연동된 캐릭터"; // Placeholder for now
                }
            }
            catch { }
            return null;
        }
    }
}
