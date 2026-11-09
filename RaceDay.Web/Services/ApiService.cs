
using System.Net.Http.Json;

namespace RaceDay.Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            return await _httpClient.GetFromJsonAsync<T>(endpoint);
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(
            string endpoint, TRequest data)
        {
            var response = await _httpClient.PostAsJsonAsync(
                endpoint, data);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                throw new HttpRequestException(
                    $"API returned {(int)response.StatusCode}: {error}",
                    null,
                    response.StatusCode);
            }

            return await response.Content.ReadFromJsonAsync<TResponse>();
        }

        public async Task PutAsync<TRequest>(
            string endpoint, TRequest data)
        {
            var response = await _httpClient.PutAsJsonAsync(
                endpoint, data);

            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint);

            response.EnsureSuccessStatusCode();
        }
        public async Task PatchAsync<TRequest>(string endpoint, TRequest data)
        {
            var response = await _httpClient.PatchAsJsonAsync(endpoint, data);
            response.EnsureSuccessStatusCode();
        }
    }
}
