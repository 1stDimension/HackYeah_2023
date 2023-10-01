using Microsoft.EntityFrameworkCore.Design;

namespace HackYeah.Backend.Keystore.Services;

public class DesignTimeKeystoreContextProvider : IDesignTimeDbContextFactory<KeystoreContext>
{
    public KeystoreContext CreateDbContext(string[] args)
        => new();
}
