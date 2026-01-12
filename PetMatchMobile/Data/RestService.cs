using System.Text.Json;
using System.Text;
using PetMatchMobile.Models;
using Microsoft.Maui.Devices;

namespace PetMatchMobile.Data
{
    public class RestService
    {
        HttpClient _client;
        JsonSerializerOptions _options;
        // ATENȚIE: 10.0.2.2 pentru Android Emulator, localhost pentru Windows
        string BaseUrl = DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:7198" : "https://localhost:7198";

        public RestService()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (m, c, ch, e) => true;
            _client = new HttpClient(handler);
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var json = JsonSerializer.Serialize(new { Email = email, Password = password });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Stergem try/catch de aici ca să vedem eroarea în LoginPage
            var response = await _client.PostAsync($"{BaseUrl}/api/auth/login", content);

            // Verificăm ce zice serverul
            if (!response.IsSuccessStatusCode)
            {
                // Opțional: Poți vedea în debug exact ce cod dă (ex: 500, 404)
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Eroare Server: {response.StatusCode} - {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }
        public async Task<bool> RegisterAsync(string email, string password)
        {
            var json = JsonSerializer.Serialize(new { Email = email, Password = password });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{BaseUrl}/api/auth/register", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Animal>> GetAnimalsAsync()
        {
            try
            {
                var response = await _client.GetAsync($"{BaseUrl}/api/Animals");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<Animal>>(content, _options);
                }
            }
            catch { }
            return new List<Animal>();
        }

        public async Task<bool> SendAdoptionRequestAsync(int animalID, string email)
        {
            try
            {
                var requestData = new { AnimalId = animalID, UserEmail = email };
                var json = JsonSerializer.Serialize(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // ATENȚIE: URL-ul s-a schimbat puțin, acum include "AdoptionApi"
                var response = await _client.PostAsync($"{BaseUrl}/api/AdoptionApi/request", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare: {ex.Message}");
                return false;
            }
        }
    }
}