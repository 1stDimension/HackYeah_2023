﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HackYeah.Backend.Keystore.Data;

namespace HackYeah.Backend.Keystore.Crypto;

public static class KeyParser
{
    public static async Task<KeyPair> ParseKeyAsync(Stream input, KeyType type, uint size, CancellationToken cancellationToken)
    {
        return await (type switch
        {
            KeyType.AES => ParseAesKeyAsync(input, size, cancellationToken),
            KeyType.RSA => ParseRsaKeyAsync(input, size, cancellationToken),
            KeyType.ECDSA => ParseEcdsaKeyAsync(input, size, cancellationToken),
            _ => ParseInvalidKey()
        });
    }

    public static KeyData ExportKey(DbCryptoKeyMaterial dbKey)
    {
        return dbKey.Type switch
        {
            KeyType.AES => ExportAesKey(dbKey),
            _ => ExportInvalidKey()
        };
    }

    public static KeyData ExportAsymmetricKey(DbCryptoKeyMaterial dbKey, bool @private)
    {
        return dbKey.Type switch
        {
            KeyType.RSA => ExportRsaKey(dbKey, @private),
            KeyType.ECDSA => ExportEcdsaKey(dbKey, @private),
            _ => ExportInvalidKey()
        };
    }

    public static async Task<X509Certificate2> LoadCertificateAsync(Stream input, CancellationToken cancellationToken)
    {
        var pem = "";
        using (var sr = new StreamReader(input, Encoding.UTF8))
            pem = await sr.ReadToEndAsync(cancellationToken);

        var cert = pem.Contains("PRIVATE KEY")
            ? X509Certificate2.CreateFromPem(pem, pem)
            : X509Certificate2.CreateFromPem(pem);

        return cert;
    }

    public static KeyPair ExtractKeyPair(X509Certificate2 cert, out KeyType type, out uint size)
    {
        type = KeyType.Unknown;
        size = 0;

        var kalg = new Oid(cert.GetKeyAlgorithm()).FriendlyName;
        if (kalg == "RSA")
        {
            type = KeyType.RSA;
            size = (uint)cert.GetRSAPublicKey().KeySize;

            using var rsa = cert.HasPrivateKey
                ? cert.GetRSAPrivateKey()
                : cert.GetRSAPublicKey();

            return new(cert.HasPrivateKey ? rsa.ExportRSAPrivateKey() : null, rsa.ExportRSAPublicKey());
        }
        else if (kalg == "ECC")
        {
            if (!cert.SignatureAlgorithm.FriendlyName.EndsWith("ECDSA"))
                return default;

            type = KeyType.ECDSA;
            size = (uint)cert.GetECDsaPublicKey().KeySize;

            using var ecdsa = cert.HasPrivateKey
                ? cert.GetECDsaPrivateKey()
                : cert.GetECDsaPublicKey();

            return new(cert.HasPrivateKey ? ecdsa.ExportECPrivateKey() : null, ecdsa.ExportSubjectPublicKeyInfo());
        }
        else
            return default;
    }

    private static Task<KeyPair> ParseInvalidKey()
        => Task.FromResult<KeyPair>(default);

    private static KeyData ExportInvalidKey()
        => default;

    private static async Task<KeyPair> ParseAesKeyAsync(Stream input, uint size, CancellationToken cancellationToken)
    {
        if (size < 1)
            return default;

        var k = new byte[size / 8];
        await input.ReadAsync(k, cancellationToken);

        return new(k, k);
    }

    private static KeyData ExportAesKey(DbCryptoKeyMaterial dbKey)
    {
        var aesKey = Convert.ToBase64String(dbKey.PrivateKey);

        return new()
        {
            Id = dbKey.Id,
            Name = dbKey.Name,
            Type = dbKey.Type,
            Size = dbKey.Size,
            Data = aesKey
        };
    }

    private static async Task<KeyPair> ParseRsaKeyAsync(Stream input, uint size, CancellationToken cancellationToken)
    {
        var pem = "";
        using (var sr = new StreamReader(input, Encoding.UTF8))
            pem = await sr.ReadToEndAsync(cancellationToken);

        using var rsa = RSA.Create();
        rsa.ImportFromPem(pem);

        return new(rsa.ExportRSAPrivateKey(), rsa.ExportRSAPublicKey());
    }

    private static KeyData ExportRsaKey(DbCryptoKeyMaterial dbKey, bool @private)
    {
        if (dbKey.PrivateKey is null && @private)
            return default;

        using var rsa = RSA.Create();

        if (dbKey.PrivateKey is null)
            rsa.ImportRSAPublicKey(dbKey.PublicKey, out _);
        else
            rsa.ImportRSAPrivateKey(dbKey.PrivateKey, out _);

        var pem = @private
            ? rsa.ExportRSAPrivateKeyPem()
            : rsa.ExportRSAPublicKeyPem();

        return new()
        {
            Id = dbKey.Id,
            Name = dbKey.Name,
            Type = dbKey.Type,
            Size = dbKey.Size,
            Data = pem
        };
    }

    private static async Task<KeyPair> ParseEcdsaKeyAsync(Stream input, uint size, CancellationToken cancellationToken)
    {
        var pem = "";
        using (var sr = new StreamReader(input, Encoding.UTF8))
            pem = await sr.ReadToEndAsync(cancellationToken);

        using var ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(pem);

        return new(ecdsa.ExportECPrivateKey(), ecdsa.ExportSubjectPublicKeyInfo());
    }

    private static KeyData ExportEcdsaKey(DbCryptoKeyMaterial dbKey, bool @private)
    {
        if (dbKey.PrivateKey is null && @private)
            return default;

        using var ecdsa = ECDsa.Create();

        if (dbKey.PrivateKey is null)
            ecdsa.ImportSubjectPublicKeyInfo(dbKey.PublicKey, out _);
        else
            ecdsa.ImportECPrivateKey(dbKey.PrivateKey, out _);

        var pem = @private
            ? ecdsa.ExportECPrivateKeyPem()
            : ecdsa.ExportSubjectPublicKeyInfoPem();

        return new()
        {
            Id = dbKey.Id,
            Name = dbKey.Name,
            Type = dbKey.Type,
            Size = dbKey.Size,
            Data = pem
        };
    }
}
