using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace InvoiceApi.Services
{
    public class ExchangeRateApi : IExchangeService
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly string ApiKey = "a4944b479e05c7910fa11d09";

        public ExchangeRateApi(HttpClient httpClient, ILoggerFactory logger)
        {
            _httpClient = httpClient;
            _logger = logger.CreateLogger(nameof(ExchangeRateApi));
        }

        public async Task<decimal?> GetRateAsync(string from, string to)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
                return null;
            if (string.Equals(from, to, StringComparison.OrdinalIgnoreCase))
                return 1;

            ApiPairResponse pair = null;
            try
            {
                using var response = await _httpClient.GetAsync($"https://v6.exchangerate-api.com/v6/{ApiKey}/pair/{from}/{to}");
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                pair = JsonSerializer.Deserialize<ApiPairResponse>(responseString);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                pair = null;
            }

            if (pair == null || pair.result == "error")
                return null;
            return pair.conversion_rate;
        }

        private class ApiPairResponse
        {
            public string result { get; set; }
            public string base_code { get; set; }
            public string target_code { get; set; }
            public decimal? conversion_rate { get; set; }
        }
    }
}
