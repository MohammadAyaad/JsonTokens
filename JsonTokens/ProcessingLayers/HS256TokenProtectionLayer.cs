using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using JsonTokens.ProcessingLayers;

namespace JsonTokens.ProcessingLayers;

public class HS256TokenProtectionLayer : ITokenProcessorLayer
{
    public (string token, string secret) FromString(string token, string secret)
    {
        (string secretKey, byte[] hashKey) = ParseSecret(secret);
        (string input, byte[] hash) = ParseHashProtectedString(token);
        byte[] hash2 = HMACSHA256.HashData(hashKey, Encoding.UTF8.GetBytes(input));
        for(int i = 0;i < hash.Length;i++) if(hash[i]!= hash2[i]) throw new ArgumentException("Invalid token", nameof(token));  
        return (input, secretKey);
    }

    public (string token, string secret) ToString(string input, string secret)
    {
        byte[] key = RandomNumberGenerator.GetBytes(32);
        byte[] hash = HMACSHA256.HashData(key, Encoding.UTF8.GetBytes(input));

        return (MakeHashProtectedString(input,hash),MakeSecret(secret,key));
    }
    private string MakeHashProtectedString(string input, byte[] hash)
    {
        return $"{input}.{Base64Converter.ToBase64String(hash)}";
    }
    private (string input, byte[] hash) ParseHashProtectedString(string input)
    {
        string[] parts = input.Split('.');
        if (parts.Length!= 2)
            throw new ArgumentException("Invalid hash protected string", nameof(input));
        byte[] hash = Base64Converter.FromBase64String(parts[1]);
        return (parts[0], hash);
    }
    private static string MakeSecret(string secret, byte[] hashKey)
    {
        return $"{secret}.{Base64Converter.ToBase64String(hashKey)}";
    }
    private static (string secret, byte[] hashKey) ParseSecret(string secret)
    {
        string[] keys = secret.Split('.');
        if (keys.Length!= 2)
            throw new ArgumentException("Invalid hash protected string");
        return (keys[0], Base64Converter.FromBase64String(keys[1]));
    }
}
