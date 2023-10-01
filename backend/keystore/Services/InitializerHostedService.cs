using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace HackYeah.Backend.Keystore.Services;

public sealed class InitializerHostedService : IHostedService
{
    private readonly KeystoreContext _db;

    public InitializerHostedService(KeystoreContext db)
    {
        this._db = db;
    }

    public async Task StartAsync(CancellationToken cancellationToken) 
        => await this._db.Database.MigrateAsync(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken) 
        => Task.CompletedTask;
}
