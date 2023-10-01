using HackYeah.Backend.Keystore.Data;
using HackYeah.Backend.Keystore.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HackYeah.Backend.Keystore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var config = new ConfigurationBuilder()
            .AddConfiguration(builder.Configuration)
            .AddEnvironmentVariables("HACKYEAH:")
            .AddCommandLine(args)
            .Build();

        // Add services to the container.
        builder.Services.AddOptions<Config>()
            .Bind(config)
            .ValidateDataAnnotations();

        builder.Services.AddDbContext<KeystoreContext>(ServiceLifetime.Singleton);

        builder.Services.AddTransient<CryptoKeyRepository>()
            .AddTransient<CertificateRepository>();

        builder.Services.AddHostedService<InitializerHostedService>();

        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            // here be dragons
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
