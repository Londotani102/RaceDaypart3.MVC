using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RaceDay.MVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("RaceDayAPI");
        }

        // GET
        public async Task<T?> GetAsync<T>(
            string endpoint, string? token = null)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, endpoint);

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            using var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>();
        }

        // POST
        public async Task<HttpResponseMessage> PostAsync<T>(
            string endpoint, T data, string? token = null)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post, endpoint)
            {
                Content = JsonContent.Create(data)
            };

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return await _httpClient.SendAsync(request);
        }

        // PUT
        public async Task<HttpResponseMessage> PutAsync<T>(
            string endpoint, T data, string? token = null)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Put, endpoint)
            {
                Content = JsonContent.Create(data)
            };

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return await _httpClient.SendAsync(request);
        }

        // DELETE
        public async Task<HttpResponseMessage> DeleteAsync(
            string endpoint, string? token = null)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Delete, endpoint);

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return await _httpClient.SendAsync(request);
        }
    }
}