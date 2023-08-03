using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyDev.BinanceApi.Data;

namespace Mydev.BinanceApi
{
    class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<CryptoCurrencyDb>();
                db.Database.EnsureCreated();
            }

            app.UseRouting();

            // Define your endpoints directly here
            app.MapGet("/cryptoCurrency", async (ICryptoCurrencyRepository repository) =>
                Results.Ok(await repository.GetCryptoCurrenciesAsync()));
            app.MapGet("/cryptoCurrency/{id}", async (int id, ICryptoCurrencyRepository repository) =>
            await repository.GetCryptoCurrencyAsync(id) is CryptoCurrency cryptoCurrency
            ? Results.Ok(cryptoCurrency)
            : Results.NotFound());

            app.MapPost("/cryptoCurrency", async ([FromBody] CryptoCurrency cryptoCurrency, ICryptoCurrencyRepository repository) =>         
            {
                await repository.InsertCryptoCurrencyAsync(cryptoCurrency);
                await repository.SaveAsync();
                return Results.Created($"/cryptoCurrency/{cryptoCurrency.Id}", cryptoCurrency);
            });

            app.MapPut("/cryptoCurrency", async ([FromBody] CryptoCurrency cryptoCurrency, ICryptoCurrencyRepository repository) =>
            {
                await repository.UpdateCryptoCurrencyAsync(cryptoCurrency);
                await repository.SaveAsync();
                return Results.NoContent();
            });
                
           
            app.MapDelete("/cryptoCurrency/{id}", async (int id, ICryptoCurrencyRepository repository) => 
            {
                await repository.DeleteCryptoCurrencyAsync(id);
                await repository.SaveAsync();
                return Results.NoContent();
            });

            app.UseHttpsRedirection();

            app.Run();
        }
    }

}


