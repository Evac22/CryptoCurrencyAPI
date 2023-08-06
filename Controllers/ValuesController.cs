using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyDev.BinanceApi.ApiClients;
using MyDev.BinanceApi.Data;

namespace MyDev.BinanceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CryptoCurrencyController : ControllerBase
    {
        private readonly BinanceApiClient _binanceApiClient;
        private readonly ICryptoCurrencyRepository _cryptoCurrencyRepository;

        public CryptoCurrencyController(BinanceApiClient binanceApiClient, ICryptoCurrencyRepository cryptoCurrencyRepository)
        {
            _binanceApiClient = binanceApiClient;
            _cryptoCurrencyRepository = cryptoCurrencyRepository;
        }

        [HttpGet("cryptocurrencies")]
        public async Task<IActionResult> GetCryptoCurrencies()
        {
            var cryptoCurrencies = await _binanceApiClient.GetCryptoCurrenciesAsync();
            return Ok(cryptoCurrencies);
        }

        [HttpGet("tradingpairs")]
        public async Task<IActionResult> GetTradingPairs()
        {
            var tradingPairs = await _binanceApiClient.GetTradingPairsAsync();
            return Ok(tradingPairs);
        }

        [HttpGet("tradingvolume")]
        public async Task<IActionResult> GetTradingVolume()
        {
            var tradingVolume = await _binanceApiClient.GetTradingVolumeAsync();
            return Ok(tradingVolume);
        }

        [HttpGet("orderbook/{symbol}")]
        public async Task<IActionResult> GetOrderBook(string symbol)
        {
            var orderBook = await _binanceApiClient.GetOrderBookAsync(symbol);
            return Ok(orderBook);
        }
    }
}
