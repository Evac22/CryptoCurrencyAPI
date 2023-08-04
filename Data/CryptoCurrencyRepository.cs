using SQLitePCL;

namespace MyDev.BinanceApi.Data
{
    public class CryptoCurrencyRepository : ICryptoCurrencyRepository
    {
        private readonly CryptoCurrencyDb _context;

        public CryptoCurrencyRepository(CryptoCurrencyDb context)
        {
            _context = context;
        }

        public Task<List<CryptoCurrency>> GetCryptoCurrenciesAsync(string name) =>
             _context.cryptoCurrencies.Where(c => c.Symbol.Contains(name)).ToListAsync();
        

        public Task<List<CryptoCurrency>> GetCryptoCurrenciesAsync() => _context.cryptoCurrencies.ToListAsync();


        public async Task<CryptoCurrency> GetCryptoCurrencyAsync(int cryptoCurrencyId) =>
           await _context.cryptoCurrencies.FindAsync(new object[] { cryptoCurrencyId });


        public async Task InsertCryptoCurrencyAsync(CryptoCurrency cryptoCurrency) => await _context.cryptoCurrencies.AddAsync(cryptoCurrency);
       

        public async Task UpdateCryptoCurrencyAsync(CryptoCurrency cryptoCurrency)
        {
            var cryptoCurrencyFromDb = await _context.cryptoCurrencies.FindAsync(new object[] { cryptoCurrency.Id });
            if (cryptoCurrencyFromDb == null) return;           
            cryptoCurrencyFromDb.Price = cryptoCurrency.Price;
            cryptoCurrencyFromDb.Symbol = cryptoCurrency.Symbol;
        }

        public async Task DeleteCryptoCurrencyAsync(int cryptoCurrencyId)
        {
            var cryptoCurrencyFromDb = await _context.cryptoCurrencies.FindAsync(new object[] { cryptoCurrencyId });
            if (cryptoCurrencyFromDb == null) return;
            _context.cryptoCurrencies.Remove(cryptoCurrencyFromDb);
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();
        
        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if(disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;

        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        
    }
}
