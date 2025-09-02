namespace ignivault.Services
{
    public class VaultService
    {
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
    }
}
