using System.Security.Cryptography;

namespace ignivault.API.Security
{
    public class Crypt
    {
        private const int KeySize = 32;
        private const int SaltSize = 16;
        private const int Iterations = 100_000;

        // Generate a new random key per user (for encrypting vault items)
        public byte[] GenerateRandomKey()
        {
            byte[] key = new byte[KeySize];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(key);
            return key;
        }

        // Generate salt for PBKDF2
        public byte[] GenerateSalt()
        {
            byte[] salt = new byte[SaltSize];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return salt;
        }

        // Derive key from password + salt
        public byte[] DeriveKey(string password, byte[] salt)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            return deriveBytes.GetBytes(KeySize);
        }

        // Encrypt data with AES256 (returns ciphertext + IV)
        public (byte[] Ciphertext, byte[] IV) Encrypt(byte[] plaintext, byte[] key)
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor(key, aes.IV);
            byte[] ciphertext = encryptor.TransformFinalBlock(plaintext, 0, plaintext.Length);

            return (ciphertext, aes.IV);
        }

        // Decrypt data with AES256
        public byte[] Decrypt(byte[] ciphertext, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(key, aes.IV);
            return decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
        }

        // Encrypt the master key with a password-derived key
        public (byte[] EncryptedMasterKey, byte[] IV) EncryptMasterKey(byte[] masterKey, string password, byte[] salt)
        {
            byte[] passwordKey = DeriveKey(password, salt);
            return Encrypt(masterKey, passwordKey);
        }

        // Decrypt the master key
        public byte[] DecryptMasterKey(byte[] encryptedMasterKey, string password, byte[] salt, byte[] iv)
        {
            byte[] passwordKey = DeriveKey(password, salt);
            return Decrypt(encryptedMasterKey, passwordKey, iv);
        }
    }
}
