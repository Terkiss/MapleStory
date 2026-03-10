using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System;

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

        public async Task<List<NexonCharacter>?> GetAccountCharactersAsync(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey)) return null;

            using var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/maplestory/v1/character/list");
            request.Headers.Add("x-nxopen-api-key", apiKey);

            try
            {
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var rawJson = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[NexonApiService] Raw JSON: {rawJson}");

                    try
                    {
                        using var doc = System.Text.Json.JsonDocument.Parse(rawJson);
                        var root = doc.RootElement;
                        var allCharacters = new List<NexonCharacter>();

                        if (root.TryGetProperty("account_list", out var accountList) && accountList.ValueKind == System.Text.Json.JsonValueKind.Array)
                        {
                            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            foreach (var account in accountList.EnumerateArray())
                            {
                                if (account.TryGetProperty("character_list", out var characterList) && characterList.ValueKind == System.Text.Json.JsonValueKind.Array)
                                {
                                    var characters = System.Text.Json.JsonSerializer.Deserialize<List<NexonCharacter>>(characterList.GetRawText(), options);
                                    if (characters != null)
                                    {
                                        allCharacters.AddRange(characters);
                                    }
                                }
                            }
                        }

                        if (allCharacters.Count > 0)
                        {
                            SortCharactersByLevel(allCharacters);
                            Console.WriteLine($"[NexonApiService] Successfully collected and sorted {allCharacters.Count} characters.");
                            return allCharacters;
                        }
                        else
                        {
                            Console.WriteLine("[NexonApiService] No characters found in any account in the account_list.");
                        }
                    }
                    catch (Exception jsonEx)
                    {
                        Console.WriteLine($"[NexonApiService] JSON Parse Error: {jsonEx.Message}");
                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[NexonApiService] API Error ({response.StatusCode}): {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NexonApiService] Global Exception: {ex.Message}");
            }
            return null;
        }

        private void SortCharactersByLevel(List<NexonCharacter> characters)
        {
            // Insertion Sort (Descending by Level)
            for (int i = 1; i < characters.Count; i++)
            {
                var key = characters[i];
                int j = i - 1;

                while (j >= 0 && characters[j].CharacterLevel < key.CharacterLevel)
                {
                    characters[j + 1] = characters[j];
                    j = j - 1;
                }
                characters[j + 1] = key;
            }
        }
    }

    public class NexonCharacterListResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("account_character")]
        public List<NexonCharacter>? AccountCharacter { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("account_characters")]
        public List<NexonCharacter>? AccountCharacters { get; set; }
    }

    public class NexonCharacter
    {
        [System.Text.Json.Serialization.JsonPropertyName("ocid")]
        public string Ocid { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("character_name")]
        public string CharacterName { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("world_name")]
        public string WorldName { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("character_class")]
        public string CharacterClass { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("character_level")]
        public int CharacterLevel { get; set; }
    }
}
