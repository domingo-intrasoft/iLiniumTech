using System.Security.Cryptography;
using System.Text;

namespace iLiniumTech.Backend.Infrastructure.Polizas.Connections;

public sealed class AppBuilderConnectionValueProtector(string? encryptionKey)
{
    private static readonly byte[] SaltBytes = [17, 25, 83, 20, 7, 85, 22, 8];

    public string DecryptData(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(encryptionKey))
        {
            return value ?? string.Empty;
        }

        Span<byte> buffer = stackalloc byte[value.Length];
        if (!Convert.TryFromBase64String(value, buffer, out _))
        {
            return value;
        }

        try
        {
            var cipherBytes = Convert.FromBase64String(value);
            var keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(encryptionKey));

            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            var derivedBytes = Rfc2898DeriveBytes.Pbkdf2(
                keyBytes,
                SaltBytes,
                1000,
                HashAlgorithmName.SHA1,
                (aes.KeySize / 8) + (aes.BlockSize / 8));
            aes.Key = derivedBytes[..(aes.KeySize / 8)];
            aes.IV = derivedBytes[(aes.KeySize / 8)..];
            aes.Mode = CipherMode.CBC;

            using var input = new MemoryStream(cipherBytes);
            using var cryptoStream = new CryptoStream(input, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var output = new MemoryStream();
            cryptoStream.CopyTo(output);

            CryptographicOperations.ZeroMemory(derivedBytes);
            return Encoding.UTF8.GetString(output.ToArray());
        }
        catch (CryptographicException)
        {
            return value;
        }
        catch (FormatException)
        {
            return value;
        }
    }
}
