using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyDev.BinanceApi;
using MyDev.BinanceApi.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Mydev.BinanceApi
{
    class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<CryptoCurrencyDb>(options =>
            {
                options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
            });

            builder.Services.AddScoped<ICryptoCurrencyRepository, CryptoCurrencyRepository>();

            builder.Services.AddRouting();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            if(app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<CryptoCurrencyDb>();
                db.Database.EnsureCreated();
            }

            app.UseRouting();

            // Define your endpoints directly here
            app.MapGet("/cryptoCurrency", async (ICryptoCurrencyRepository repository) =>
                Results.Extensions.Xml(await repository.GetCryptoCurrenciesAsync()))
                .Produces<List<CryptoCurrency>>(StatusCodes.Status200OK)
                .WithName("GetAllCryptoCurrency")
                .WithTags("Getters");
            app.MapGet("/cryptoCurrency/{id}", async (int id, ICryptoCurrencyRepository repository) =>
            await repository.GetCryptoCurrencyAsync(id) is CryptoCurrency cryptoCurrency
            ? Results.Ok(cryptoCurrency)
            : Results.NotFound())
                .Produces<CryptoCurrency>(StatusCodes.Status200OK)
                .WithName("GetCryptoCurrentcy")
                .WithTags("Getters");

            app.MapPost("/cryptoCurrency", async ([FromBody] CryptoCurrency cryptoCurrency, ICryptoCurrencyRepository repository) =>
            {
                await repository.InsertCryptoCurrencyAsync(cryptoCurrency);
                await repository.SaveAsync();
                return Results.Created($"/cryptoCurrency/{cryptoCurrency.Id}", cryptoCurrency);
            })
              .Accepts<CryptoCurrency>("application/json")
              .Produces<CryptoCurrency>(StatusCodes.Status201Created)
              .WithName("CreateCryptoCurrency")
              .WithTags("Creaters");


            app.MapPut("/cryptoCurrency", async ([FromBody] CryptoCurrency cryptoCurrency, ICryptoCurrencyRepository repository) =>
            {
                await repository.UpdateCryptoCurrencyAsync(cryptoCurrency);
                await repository.SaveAsync();
                return Results.NoContent();
            })
              .Accepts<CryptoCurrency>("application/json")
              .WithName("UpdateCryptoCurrency")
              .WithTags("Updaters");


            app.MapDelete("/cryptoCurrency/{id}", async (int id, ICryptoCurrencyRepository repository) =>
            {
                await repository.DeleteCryptoCurrencyAsync(id);
                await repository.SaveAsync();
                return Results.NoContent();
            })
              .WithName("DeleteCryptoCurrency")
              .WithTags("Deleters");

            app.MapGet("/cryptoCurrency/search/name/{query}",
                async (string query, ICryptoCurrencyRepository repository) =>
                    await repository.GetCryptoCurrenciesAsync(query) is IEnumerable<CryptoCurrency> cryptos
                        ? Results.Ok(cryptos)
                        : Results.NotFound(Array.Empty<CryptoCurrency>()))
                .Produces<List<CryptoCurrency>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("SearchCryptoCurrenty")
                .WithTags("Getters")
                .ExcludeFromDescription();

            app.MapGet("/cryptoCurrency/search/crypto/{cryptoSearch}",
                async (CryptoSearch cryptoSearch, ICryptoCurrencyRepository repository) =>
                   await repository.GetCryptoCurrenciesAsync(cryptoSearch) is IEnumerable<CryptoCurrency> cryptos
                       ? Results.Ok(cryptos)
                       : Results.NotFound(Array.Empty<CryptoCurrency>()))
                  .ExcludeFromDescription();
                
                
            app.UseHttpsRedirection();

            app.Run();
        }
    }

}
