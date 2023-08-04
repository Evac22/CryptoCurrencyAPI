namespace MyDev.BinanceApi.Data
{
    public interface ICryptoCurrencyRepository:IDisposable
    {
        Task<List<CryptoCurrency>> GetCryptoCurrenciesAsync();

        Task<List<CryptoCurrency>> GetCryptoCurrenciesAsync(string name);

        Task<List<CryptoCurrency>> GetCryptoCurrenciesAsync(CryptoSearch cryptoSearch);

        Task<CryptoCurrency> GetCryptoCurrencyAsync(int cryptoCurrencyId);

        Task InsertCryptoCurrencyAsync(CryptoCurrency cryptoCurrency);

        Task DeleteCryptoCurrencyAsync(int cryptoCurrencyId);

        Task UpdateCryptoCurrencyAsync(CryptoCurrency cryptoCurrency);

        Task SaveAsync();
    }
}
