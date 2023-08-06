using MyDev.BinanceApi.Auth;
using MyDev.BinanceApi.Data;
using MyDev.BinanceApi;
using MyDev.BinanceApi.Apis;
using MyDev.BinanceApi.ApiClients;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services);

var app = builder.Build();

Configure(app);

var apis = app.Services.GetServices<IApi>();
foreach(var api in apis)
{
    if (api is null) throw new InvalidProgramException("Api not found");
    api.Register(app);
}

app.Run();

void ConfigureServices(IServiceCollection services)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    services.AddDbContext<CryptoCurrencyDb>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
    });

    services.AddScoped<ICryptoCurrencyRepository, CryptoCurrencyRepository>();
    services.AddSingleton<ITokenService, TokenService>();
    services.AddSingleton<IUserRepository, UserRepository>();
    services.AddAuthorization();
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };
        });

    services.AddTransient<IApi, CryptoCurrencyApi>();
    services.AddTransient<IApi, AuthApi>();
    services.AddHttpClient<BinanceApiClient>(client =>
    {
        client.BaseAddress = new Uri("https://api.binance.com/api/v3");
    });
}


void Configure(WebApplication app)
{
    app.UseRouting();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();


    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CryptoCurrencyDb>();
        db.Database.EnsureCreated();
    }

   
}