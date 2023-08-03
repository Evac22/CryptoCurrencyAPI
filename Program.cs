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
            app.MapGet("/cryptoCurrency", async (CryptoCurrencyDb db) => db.cryptoCurrencies.ToListAsync());
            app.MapGet("/cryptoCurrency/{id}", async (int id, CryptoCurrencyDb db) =>
            await db.cryptoCurrencies.FirstOrDefaultAsync(c => c.Id == id) is CryptoCurrency cryptoCurrency
            ? Results.Ok(cryptoCurrency)
            : Results.NotFound());

            app.MapPost("/cryptoCurrency", async ([FromBody] CryptoCurrency cryptoCurrency, [FromServices] CryptoCurrencyDb db) =>         
            {
                db.cryptoCurrencies.Add(cryptoCurrency);
                await db.SaveChangesAsync();
                return Results.Created($"/cryptoCurrency/{cryptoCurrency.Id}", cryptoCurrency);
            });

            app.MapPut("/cryptoCurrency", async ([FromBody] CryptoCurrency cryptoCurrency, CryptoCurrencyDb db) =>
            {
                var cryptoCurrencyFromDb = await db.cryptoCurrencies.FindAsync(new object[] { cryptoCurrency.Id });
                if (cryptoCurrencyFromDb == null) return Results.NotFound();
                cryptoCurrencyFromDb.Symbol = cryptoCurrency.Symbol;
                cryptoCurrencyFromDb.Price = cryptoCurrency.Price;
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
                
           
            app.MapDelete("/cryptoCurrency/{id}", async (int id, CryptoCurrencyDb db) => 
            {
                var cryptoCurrencyFromDb = await db.cryptoCurrencies.FindAsync(new object[] { id });
                if (cryptoCurrencyFromDb == null) return Results.NotFound();
                db.cryptoCurrencies.Remove(cryptoCurrencyFromDb);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            app.UseHttpsRedirection();

            app.Run();
        }
    }

}


