using HackYeah.Backend.AsymmetricCrypto.Data;
using HackYeah.Backend.AsymmetricCrypto.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HackYeah.Backend.AsymmetricCrypto;

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
        builder.Services.Configure<KestrelServerOptions>(o => o.AllowSynchronousIO = true);

        builder.Services.AddOptions<Config>()
            .Bind(config)
            .ValidateDataAnnotations();

        builder.Services.AddTransient<FileEncryptionHandler>();

        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
