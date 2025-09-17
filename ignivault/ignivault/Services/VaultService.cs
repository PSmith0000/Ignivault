using ignivault.Data.Models.Auth;
using ignivault.Data.Models.Data;
using Microsoft.JSInterop;
using System.Text;
using System.Text.Json;

namespace ignivault.Services
{
    /// <summary>
    /// Handles client-side encryption/decryption of Vault data and master-key management.
    /// </summary>
    public class VaultService
    {
        private readonly IJSRuntime _js;
        private byte[]? _masterKey;

        public VaultService(IJSRuntime js) => _js = js ?? throw new ArgumentNullException(nameof(js));

        #region Master Key

        public void SetMasterKey(byte[] key) => _masterKey = key ?? throw new ArgumentNullException(nameof(key));
        public byte[]? GetMasterKey() => _masterKey;

        public string GetMasterKeyBase64() =>
            _masterKey != null ? Convert.ToBase64String(_masterKey) : throw new InvalidOperationException("Master key not set.");

        public void ClearMasterKey()
        {
            if (_masterKey != null) Array.Clear(_masterKey, 0, _masterKey.Length);
            _masterKey = null;
        }

        public bool IsMasterKeySet()
        {
            bool nil = (_masterKey != null);
            return nil;
        }

        public async Task<bool> DecryptMasterKeyAsync(LoginUser login, string password)
        {
            if (login == null) throw new ArgumentNullException(nameof(login));
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password cannot be empty.", nameof(password));

            try
            {
                byte[] mk = (login.EncryptedMasterKey);
                byte[] ks = (login.KeySalt);
                byte[] iv = (login.MasterIV);

                byte[] masterKey = await _js.InvokeAsync<byte[]>("Crypt.decryptMasterKey", mk, password, ks, iv);
                SetMasterKey(masterKey);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DecryptMasterKeyAsync failed: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Encryption / Decryption

        public async Task<(byte[] Ciphertext, byte[] IV)> EncryptAsync(byte[] plaintext, byte[] key)
        {
            var result = await _js.InvokeAsync<EncryptionResult>("Crypt.encrypt", plaintext, key);
            return (result.Ciphertext, result.Iv);
        }

        public async Task<byte[]> DecryptAsync(byte[] ciphertext, byte[] key, byte[] iv)
        {
            return await _js.InvokeAsync<byte[]>("Crypt.decrypt", ciphertext, key, iv);
        }

        public async Task<T?> DecryptRecordTypeAsync<T>(VaultItem item)
        {
            try
            {
                if (!IsMasterKeySet()) throw new InvalidOperationException("Master key not set.");

                byte[] cipher = (item.EncryptedData);
                byte[] iv = (item.Iv);

                byte[] decryptedBytes = await DecryptAsync(cipher, _masterKey!, iv);
                string json = Encoding.UTF8.GetString(decryptedBytes);
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DecryptRecordTypeAsync failed for item {item.Id}: {ex.Message}");
                return default;
            }
        }

        private class EncryptionResult
        {
            public byte[] Ciphertext { get; set; } = Array.Empty<byte>();
            public byte[] Iv { get; set; } = Array.Empty<byte>();
        }

        #endregion
    }
}
