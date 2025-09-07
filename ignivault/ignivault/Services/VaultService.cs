using ignivault.Data.Models.Auth;
using ignivault.Data.Models.Data;
using Microsoft.JSInterop;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System.Runtime.CompilerServices;

namespace ignivault.Services
{
    public class VaultService
    {
        private readonly IJSRuntime JS;
        public VaultService(IJSRuntime _js)
        {
            JS= _js;
            if (JS == null)
                throw new ArgumentNullException(nameof(JS));
        }

        private byte[] _masterKey;

        public void SetMasterKey(byte[] key) => _masterKey = key;

        public byte[] GetMasterKey()
        {
            if (_masterKey == null)
                Console.WriteLine("Master key not set.");
            return _masterKey;
        }

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

        public async Task<bool> DecryptMasterKey(LoginResponse login, string password)
        {
            try
            {
                var encMasterKey = (login.MasterKey);
                var keySalt = (login.keySalt);
                var masterKeyIV = (login.MasterIV);
                string b64_masterkey = await JS.InvokeAsync<string>("Crypt.decryptMasterKeyBase64", encMasterKey, password, keySalt, masterKeyIV);
                byte[] masterKey = Convert.FromBase64String(b64_masterkey);

                this.SetMasterKey(masterKey);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decrypting master key: {ex.Message}");
                return false;
            }
        }

        public async Task<(string Ciphertext, string IV)> EncryptBase64(string plaintextBase64, string keyBase64)
        {
            var result = await JS.InvokeAsync<EncryptionResult>(
                "Crypt.encryptBase64",
                plaintextBase64,
                keyBase64
            );
            return (result.Ciphertext, result.Iv);
        }

        public async Task<string> DecryptBase64(string ciphertextBase64, string keyBase64, string ivBase64) =>
        await JS.InvokeAsync<string>("Crypt.decryptBase64", ciphertextBase64, keyBase64, ivBase64);

        private class EncryptionResult
        {
            public string Ciphertext { get; set; }
            public string Iv { get; set; }
        }
    }
}
