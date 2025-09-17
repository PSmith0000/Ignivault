using ignivault.API.Core.Interface;
using System.Security.Cryptography;

namespace ignivault.API.Services
{
    public class CryptService : ICryptService
    {
        private const int KeySize = 32;   // 256 bits
        private const int SaltSize = 16;
        private const int Iterations = 100_000;
        private const int BufferSize = 81920; // 80 KB buffer

        public byte[] GenerateRandomKey()
        {
            var key = new byte[KeySize];
            RandomNumberGenerator.Fill(key);
            return key;
        }

        public byte[] GenerateSalt()
        {
            var salt = new byte[SaltSize];
            RandomNumberGenerator.Fill(salt);
            return salt;
        }

        public byte[] DeriveKey(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(KeySize);
        }

        public (byte[] Ciphertext, byte[] IV) Encrypt(byte[] plaintext, byte[] key)
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV();

            using var enc = aes.CreateEncryptor(key, aes.IV);
            var cipher = enc.TransformFinalBlock(plaintext, 0, plaintext.Length);
            return (cipher, aes.IV);
        }

        public byte[] Decrypt(byte[] ciphertext, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.IV = iv;

            using var dec = aes.CreateDecryptor(key, aes.IV);
            return dec.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
        }

        public (byte[] EncryptedMasterKey, byte[] IV) EncryptMasterKey(byte[] masterKey, string password, byte[] salt)
        {
            var passwordKey = DeriveKey(password, salt);
            return Encrypt(masterKey, passwordKey);
        }

        public byte[] DecryptMasterKey(byte[] encryptedMasterKey, string password, byte[] salt, byte[] iv)
        {
            var passwordKey = DeriveKey(password, salt);
            return Decrypt(encryptedMasterKey, passwordKey, iv);
        }

        public async Task<(byte[] IV, long BytesWritten)> EncryptStreamAsync(Stream input, Stream output, byte[] key, CancellationToken ct = default)
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV();

            using var cryptoStream = new CryptoStream(output, aes.CreateEncryptor(key, aes.IV), CryptoStreamMode.Write);
            var buffer = new byte[BufferSize];
            long total = 0;

            int read;
            while ((read = await input.ReadAsync(buffer.AsMemory(0, buffer.Length), ct)) > 0)
            {
                await cryptoStream.WriteAsync(buffer.AsMemory(0, read), ct);
                total += read;
            }

            await cryptoStream.FlushAsync(ct);
            return (aes.IV, total);
        }

        public async Task DecryptStreamAsync(Stream input, Stream output, byte[] key, byte[] iv, CancellationToken ct = default)
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var cryptoStream = new CryptoStream(input, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read);
            var buffer = new byte[BufferSize];

            int read;
            while ((read = await cryptoStream.ReadAsync(buffer.AsMemory(0, buffer.Length), ct)) > 0)
            {
                await output.WriteAsync(buffer.AsMemory(0, read), ct);
            }
            await output.FlushAsync(ct);
        }
    }
}
