#nullable disable
using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CurrencyApp
{
    internal class ExchangeRates
    {
        private const string API_KEY = "88492d728b854b3eab29bbcd";
        private const string BASE_URL = "https://v6.exchangerate-api.com/v6/";
        private static readonly HttpClient _client = new HttpClient();

        // Cache rates to avoid excessive API calls
        private Dictionary<string, double> _cachedRates = new Dictionary<string, double>();
        private string _cachedBase = "";
        private DateTime _cacheTime = DateTime.MinValue;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);

        public async Task<List<string>> GetAvailableCurrenciesAsync()
        {
            var rates = await GetRatesAsync("USD");
            var currencies = new List<string>(rates.Keys);
            currencies.Sort();
            return currencies;
        }
        public async Task<double> ConvertAsync(double amount, string fromCurrency, string toCurrency)
        {
            if (fromCurrency == toCurrency) return amount;

            var rates = await GetRatesAsync(fromCurrency);
            if (rates.ContainsKey(toCurrency))
            {
                return amount * rates[toCurrency];
            }
            throw new Exception($"Could not find rate for {toCurrency}.");
        }
        public async Task<Dictionary<string, double>> GetRatesAsync(string baseCurrency)
        {
            // Return cached if still fresh
            if (_cachedBase == baseCurrency && DateTime.Now - _cacheTime < CacheDuration)
                return _cachedRates;

            string url = $"{BASE_URL}{API_KEY}/latest/{baseCurrency}";
            var response = await _client.GetStringAsync(url);
            var result = JsonConvert.DeserializeObject<ApiResponse>(response);

            if (result == null || result.Result != "success")
                throw new Exception("Failed to fetch exchange rates from API.");

            _cachedRates = result.ConversionRates;
            _cachedBase = baseCurrency;
            _cacheTime = DateTime.Now;

            return _cachedRates;
        }

        public string GetLastUpdateTime()
        {
            return _cacheTime == DateTime.MinValue
                ? "Not yet fetched"
                : _cacheTime.ToString("dd MMM yyyy, HH:mm");
        }

    }
    public class ApiResponse
    {
        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("base_code")]
        public string BaseCode { get; set; }

        [JsonProperty("time_last_update_utc")]
        public string TimeLastUpdate { get; set; }

        [JsonProperty("conversion_rates")]
        public Dictionary<string, double> ConversionRates { get; set; }
    }
}


