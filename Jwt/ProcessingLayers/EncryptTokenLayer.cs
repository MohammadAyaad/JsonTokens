using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Jwt.ProcessingLayers
{
    public class EncryptTokenLayer : IJwtTokenProcessorLayer
    {
        public (string token, string secret) ToString(string input, string secret)

        {
            (string secretKey, string encryptionKey) = ParseSecret(secret);

            string token = Decrypt(input, encryptionKey);

            return (token, secret);
        }

        (string token, string secret) IJwtTokenProcessorLayer.FromString(string token, string secret)
        {
            (string encryptedString, string encryptionKey) = Encrypt(token);

            string secretKey = MakeSecret(secret, encryptionKey);

            return (encryptedString, secretKey);
        }



        private static string MakeSecret(string secretKey, string encryptionKey)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{secretKey}:{encryptionKey}"));
        }
        internal static (string secretKey, string encryptionKey) ParseSecret(string secret)
        {
            string[] keys = Encoding.UTF8.GetString(Convert.FromBase64String(secret)).Split(':');
            //WARNING: if the keys.length exceeds 2 elements , there is an error or the data got edited by 
            return (keys[0], $"{keys[1]}:{keys[2]}");
        }






        private static (string encryptedString, string encryptionKey) Encrypt(string plainText)
        {
            using (var aes = new AesCryptoServiceProvider())
            {
                aes.GenerateKey();
                aes.GenerateIV();


                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        var encryptedBytes = msEncrypt.ToArray();
                        var encryptedString = Convert.ToBase64String(encryptedBytes);
                        var secretKey = Convert.ToBase64String(aes.Key);
                        var iv = Convert.ToBase64String(aes.IV);

                        string secret = $"{secretKey}:{iv}";

                        return (encryptedString, secret);//encryptionKey = $"{secret}:{iv}"
                    }
                }
            }
        }
        private static string Decrypt(string encryptedString, string encryptionKey)
        {
            var encryptedBytes = Convert.FromBase64String(encryptedString);
            string[] keys = encryptionKey.Split(':');
            var key = Convert.FromBase64String(keys[0]);
            var iv = Convert.FromBase64String(keys[1]);

            using (var aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                aes.IV = iv;

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            var decryptedText = srDecrypt.ReadToEnd();
                            return decryptedText;
                        }
                    }
                }
            }
        }
        private static byte[] generateKey()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] secretKey = new byte[32]; // 256 bits
                rng.GetBytes(secretKey);

                var secretKeyBase64 = Convert.ToBase64String(secretKey);

                return secretKey;
            }
        }

    }
}
