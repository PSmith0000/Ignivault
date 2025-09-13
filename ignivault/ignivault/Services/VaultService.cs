using ignivault.Data.Models.Auth;
using ignivault.Data.Models.Data;
using ignivault.Pages;
using Microsoft.JSInterop;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ignivault.Services
{
    /// <summary>
    /// Handles client-side encryption/decryption of Vault data and master-key management.
    /// JS now works on raw byte arrays; C# handles Base64 conversions.
    /// </summary>
    public class VaultService
    {
        private readonly IJSRuntime _js;
        private byte[]? _masterKey;

        public VaultService(IJSRuntime js)
        {
            _js = js ?? throw new ArgumentNullException(nameof(js));
        }

        #region Master Key Management

        public void SetMasterKey(byte[] key) => _masterKey = key ?? throw new ArgumentNullException(nameof(key));

        public byte[]? GetMasterKey() => _masterKey;

        public string GetMasterKeyBase64()
        {
            if (_masterKey == null)
                throw new InvalidOperationException("Master key not set.");
            return Convert.ToBase64String(_masterKey);
        }

        public void ClearMasterKey()
        {
            if (_masterKey != null)
                Array.Clear(_masterKey, 0, _masterKey.Length);
            _masterKey = null;
        }

        public bool IsMasterKeySet() => _masterKey != null;

        /// <summary>
        /// Decrypt and cache the master key using the supplied password.
        /// JS returns raw bytes; C# converts to byte[].
        /// </summary>
        public async Task<bool> DecryptMasterKeyAsync(LoginUser login, string password)
        {
            try
            {
                byte[] mk = Convert.FromBase64String(login.EncryptedMasterKey);
                byte[] ks = Convert.FromBase64String(login.KeySalt);
                byte[] iv = Convert.FromBase64String(login.MasterIV);
                byte[] masterKey = await _js.InvokeAsync<byte[]>("Crypt.decryptMasterKey", mk, password, ks, iv);          

                SetMasterKey(masterKey);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decrypting master key: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Encryption / Decryption

        /// <summary>
        /// Encrypts a plaintext byte array with a key byte array. Returns raw ciphertext and IV.
        /// </summary>
        public async Task<(byte[] Ciphertext, byte[] IV)> EncryptAsync(byte[] plaintext, byte[] key)
        {
            var result = await _js.InvokeAsync<EncryptionResult>("Crypt.encrypt", plaintext, key);

            return (result.Ciphertext, result.Iv);
        }

        /// <summary>
        /// Decrypts a ciphertext byte array with a key and IV byte arrays. Returns raw plaintext.
        /// </summary>
        public async Task<byte[]> DecryptAsync(byte[] ciphertext, byte[] key, byte[] iv)
        {
            return await _js.InvokeAsync<byte[]>("Crypt.decrypt", ciphertext, key, iv);
        }

        /// <summary>
        /// Decrypts a VaultItem into the specified record type.
        /// JS returns raw bytes; C# converts to UTF-8 string and deserializes.
        /// </summary>
        public async Task<T?> DecryptRecordTypeAsync<T>(VaultItem item)
        {
            try
            {
                if (!IsMasterKeySet())
                    throw new InvalidOperationException("Master key not set.");

                byte[] mk = Convert.FromBase64String(item.EncryptedData);
                byte[] iv = Convert.FromBase64String(item.IV);
                byte[] decryptedBytes = await DecryptAsync(mk, _masterKey!, iv);

                string decryptedJson = Encoding.UTF8.GetString(decryptedBytes);
                return JsonSerializer.Deserialize<T>(decryptedJson);
            }
            catch (Exception)
            {
                Console.WriteLine($"Error decrypting item ID {item.Id}, {item.EncryptedData}, {item.IV}");
            }

            return default;
        }

        #endregion

        private class EncryptionResult
        {
            public byte[] Ciphertext { get; set; } = Array.Empty<byte>();
            public byte[] Iv { get; set; } = Array.Empty<byte>();
        }
    }
}
