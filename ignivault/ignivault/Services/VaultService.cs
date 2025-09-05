using ignivault.Data.Models.Auth;
using ignivault.Data.Models.Data;
using Microsoft.JSInterop;
using System.Runtime.CompilerServices;

namespace ignivault.Services
{
    public class VaultService
    {
        private readonly IJSRuntime JS;
        public VaultService(IJSRuntime _js)
        {
            _js = JS;
        }

        private byte[] _masterKey;

        public void SetMasterKey(byte[] key) => _masterKey = key;

        public byte[] GetMasterKey()
        {
            if (_masterKey == null)
                throw new InvalidOperationException("Master key not set.");
            return _masterKey;
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
            catch
            {
                return false;
            }
        }
    }
}
