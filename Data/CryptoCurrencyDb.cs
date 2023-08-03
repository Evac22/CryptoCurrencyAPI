namespace MyDev.BinanceApi.Data
{
    public class CryptoCurrencyDb : DbContext
    {
        public CryptoCurrencyDb(DbContextOptions<CryptoCurrencyDb> options) : base(options) { }

        public DbSet<CryptoCurrency> cryptoCurrencies => Set<CryptoCurrency>();
    }
}
