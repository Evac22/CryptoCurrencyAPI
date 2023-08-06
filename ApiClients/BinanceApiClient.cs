using MyDev.BinanceApi.Data;
using System.Text.Json;

namespace MyDev.BinanceApi.ApiClients
{
    public class BinanceApiClient
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.binance.com/api/v3";


        public BinanceApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        // Метод для получения списка криптовалют с их текущими ценами
        public async Task<List<CryptoCurrency>> GetCryptoCurrenciesAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/ticker/price");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var cryptoCurrencies = JsonSerializer.Deserialize<List<CryptoCurrency>>(json);
            return cryptoCurrencies;
        }

        // Метод для получения информации о торговых парами (торговые пары и их характеристики)
        public async Task<TradingPair[]> GetTradingPairsAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/exchangeInfo");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var exchangeInfo = JsonSerializer.Deserialize<ExchangeInfo>(json);
            return exchangeInfo.Symbols;
        }

        // Метод для получения объема торгов за последние 24 часа
        public async Task<TradingVolume> GetTradingVolumeAsync()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/ticker/24hr");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var tradingVolume = JsonSerializer.Deserialize<TradingVolume>(json);
            return tradingVolume;
        }

        // Метод для получения глубины рынка (первые 10 ордеров на покупку и продажу) для заданной торговой пары
        public async Task<OrderBook> GetOrderBookAsync(string symbol)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/depth?symbol={symbol}&limit=10");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var orderBook = JsonSerializer.Deserialize<OrderBook>(json);
            return orderBook;
        }

        // Классы для десериализации ответов Binance API
        public class ExchangeInfo
        {
            public TradingPair[] Symbols { get; set; }           
        }

        public class TradingPair
        {
            public string Symbol { get; set; }           
        }

        public class TradingVolume
        {
            public decimal Volume { get; set; }
        }

        public class OrderBook
        {
            public decimal[][] Bids { get; set; }
            public decimal[][] Asks { get; set; }
        }
    }

}
