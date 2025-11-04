using System.Security.Cryptography;
using System.Text;
using ItemManager.Models;

namespace ItemManager.Services;

public class TotpService
{
    private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public bool ValidateCode(User user, string code, int tolerance = 1)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        var secretBytes = Base32Decode(user.SecretKey);
        var timestamp = DateTime.UtcNow;

        for (var offset = -tolerance; offset <= tolerance; offset++)
        {
            var comparison = GenerateCode(secretBytes, timestamp.AddSeconds(offset * 30));
            if (CryptographicEquals(code, comparison))
            {
                return true;
            }
        }

        return false;
    }

    public string BuildOtpAuthUri(User user, string issuer)
    {
        var label = Uri.EscapeDataString($"{issuer}:{user.Username}");
        var encodedIssuer = Uri.EscapeDataString(issuer);
        return $"otpauth://totp/{label}?secret={user.SecretKey}&issuer={encodedIssuer}&digits=6";
    }

    private static string GenerateCode(byte[] secret, DateTime timestamp)
    {
        var timestep = (long)(timestamp.ToUniversalTime() - UnixEpoch).TotalSeconds / 30L;
        var timestepBytes = BitConverter.GetBytes(timestep);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(timestepBytes);
        }

        using var hmac = new HMACSHA1(secret);
        var hash = hmac.ComputeHash(timestepBytes);
        var offset = hash[^1] & 0x0F;
        var binaryCode = (hash[offset] & 0x7F) << 24
                         | (hash[offset + 1] & 0xFF) << 16
                         | (hash[offset + 2] & 0xFF) << 8
                         | (hash[offset + 3] & 0xFF);

        var otp = binaryCode % 1_000_000;
        return otp.ToString("D6");
    }

    private static byte[] Base32Decode(string input)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var cleaned = input.Trim().Replace("=", string.Empty).ToUpperInvariant();

        var bits = new StringBuilder();
        foreach (var c in cleaned)
        {
            var index = alphabet.IndexOf(c);
            if (index < 0)
            {
                throw new FormatException($"Caracter Base32 inválido: {c}");
            }

            bits.Append(Convert.ToString(index, 2).PadLeft(5, '0'));
        }

        var bytes = new List<byte>();
        for (var i = 0; i + 8 <= bits.Length; i += 8)
        {
            var chunk = bits.ToString(i, 8);
            bytes.Add(Convert.ToByte(chunk, 2));
        }

        return bytes.ToArray();
    }

    private static bool CryptographicEquals(string left, string right)
    {
        if (left.Length != right.Length)
        {
            return false;
        }

        var result = 0;
        for (var i = 0; i < left.Length; i++)
        {
            result |= left[i] ^ right[i];
        }

        return result == 0;
    }
}
