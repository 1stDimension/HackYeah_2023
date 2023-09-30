using HackYeah.Backend.Keystore.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HackYeah.Backend.Keystore.Services;

public sealed class KeystoreContext : DbContext
{
    public DbSet<DbCryptoKeyMaterial> KeyMaterial { get; set; }

    private readonly string _connStr;

    public KeystoreContext(IOptions<Config> config)
    {
        var csb = new SqliteConnectionStringBuilder()
        {
            DataSource = config.Value.DatabaseFile
        };

        this._connStr = csb.ConnectionString;
    }

    internal KeystoreContext()
    {
        var csb = new SqliteConnectionStringBuilder()
        {
            DataSource = "design-time.db"
        };

        this._connStr = csb.ConnectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite(this._connStr);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbCryptoKeyMaterial>(e =>
        {
            e.ToTable("keys");

            e.Property(m => m.Id)
                .IsRequired()
                .ValueGeneratedNever()
                .HasColumnName("id");

            e.Property(m => m.Name)
                .IsRequired()
                .HasColumnName("name");

            e.Property(m => m.Type)
                .IsRequired()
                .HasColumnName("type")
                .HasConversion<string>();

            e.Property(m => m.Size)
                .IsRequired()
                .HasColumnName("size");

            e.Property(m => m.PrivateKey)
                .IsRequired()
                .HasColumnName("priv");

            e.Property(m => m.PublicKey)
                .IsRequired()
                .HasColumnName("pub");

            e.HasKey(m => m.Id)
                .HasName("pkey_key_id");

            e.HasAlternateKey(m => m.Name)
                .HasName("ukey_key_name");

            e.HasIndex(m => m.Name)
                .IsUnique()
                .HasDatabaseName("ix_key_name");

            e.HasIndex(m => m.Type)
                .HasDatabaseName("ix_key_type");

            e.HasIndex(m => m.Size)
                .HasDatabaseName("ix_key_size");
        });
    }
}
