using MyDev.BinanceApi.Data;

namespace MyDev.BinanceApi.Apis
{
    public class CryptoCurrencyApi : IApi
    {
        public void Register(WebApplication app)
        {
            app.MapGet("/cryptoCurrency", Get)
                    .Produces<List<CryptoCurrency>>(StatusCodes.Status200OK)
                    .WithName("GetAllCryptoCurrency")
                    .WithTags("Getters");

            app.MapGet("/cryptoCurrency/{id}", GetbyId)                        
                     .Produces<CryptoCurrency>(StatusCodes.Status200OK)
                     .WithName("GetCryptoCurrentcy")
                     .WithTags("Getters");

            app.MapPost("/cryptoCurrency", Post)
              .Accepts<CryptoCurrency>("application/json")
              .Produces<CryptoCurrency>(StatusCodes.Status201Created)
              .WithName("CreateCryptoCurrency")
              .WithTags("Creaters");


            app.MapPut("/cryptoCurrency", Put)
              .Accepts<CryptoCurrency>("application/json")
              .WithName("UpdateCryptoCurrency")
              .WithTags("Updaters");


            app.MapDelete("/cryptoCurrency/{id}",Delete)
              .WithName("DeleteCryptoCurrency")
              .WithTags("Deleters");

            app.MapGet("/cryptoCurrency/search/name/{query}", SearchByName)               
              .Produces<List<CryptoCurrency>>(StatusCodes.Status200OK)
              .Produces(StatusCodes.Status404NotFound)
              .WithName("SearchCryptoCurrenty")
              .WithTags("Getters")
              .ExcludeFromDescription();

            app.MapGet("/cryptoCurrency/search/crypto/{cryptoSearch}",SearchByCryptoSearch)                   
              .ExcludeFromDescription();

        }

        [Authorize]
        private async Task<IResult> Get(ICryptoCurrencyRepository repository) =>
                 Results.Extensions.Xml(await repository.GetCryptoCurrenciesAsync());

        [Authorize]
        private async Task<IResult> GetbyId(int id, ICryptoCurrencyRepository repository) =>
                 await repository.GetCryptoCurrencyAsync(id) is CryptoCurrency cryptoCurrency
                 ? Results.Ok(cryptoCurrency)
                 : Results.NotFound();

        [Authorize]
        private async Task<IResult> Post([FromBody] CryptoCurrency cryptoCurrency, ICryptoCurrencyRepository repository)
        {
            await repository.InsertCryptoCurrencyAsync(cryptoCurrency);
            await repository.SaveAsync();
            return Results.Created($"/cryptoCurrency/{cryptoCurrency.Id}", cryptoCurrency);
        }

        [Authorize]
        private async Task<IResult> Put([FromBody] CryptoCurrency cryptoCurrency, ICryptoCurrencyRepository repository)
        {
             await repository.UpdateCryptoCurrencyAsync(cryptoCurrency);
             await repository.SaveAsync();
             return Results.NoContent();
        }

        [Authorize]
        private async Task<IResult> Delete(int id, ICryptoCurrencyRepository repository)
        {
                await repository.DeleteCryptoCurrencyAsync(id);
                await repository.SaveAsync();
                return Results.NoContent();
        }

        [Authorize]
        private async Task<IResult> SearchByName(string query, ICryptoCurrencyRepository repository) =>      
                await repository.GetCryptoCurrenciesAsync(query) is IEnumerable<CryptoCurrency> cryptos
                ? Results.Ok(cryptos)
                : Results.NotFound(Array.Empty<CryptoCurrency>());

        [Authorize]
        private async Task<IResult> SearchByCryptoSearch(CryptoSearch cryptoSearch, ICryptoCurrencyRepository repository) =>
                 await repository.GetCryptoCurrenciesAsync(cryptoSearch) is IEnumerable<CryptoCurrency> cryptos
                 ? Results.Ok(cryptos)
                 : Results.NotFound(Array.Empty<CryptoCurrency>());
    }
}
