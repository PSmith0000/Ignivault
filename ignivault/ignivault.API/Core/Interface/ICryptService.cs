namespace ignivault.API.Core.Interface
{
    public interface ICryptService
    {
        byte[] GenerateRandomKey();
        byte[] GenerateSalt();
        byte[] DeriveKey(string password, byte[] salt);

        (byte[] Ciphertext, byte[] IV) Encrypt(byte[] plaintext, byte[] key);
        byte[] Decrypt(byte[] ciphertext, byte[] key, byte[] iv);

        (byte[] EncryptedMasterKey, byte[] IV) EncryptMasterKey(byte[] masterKey, string password, byte[] salt);
        byte[] DecryptMasterKey(byte[] encryptedMasterKey, string password, byte[] salt, byte[] iv);

        Task<(byte[] IV, long BytesWritten)> EncryptStreamAsync(Stream input, Stream output, byte[] key, CancellationToken ct = default);
        Task DecryptStreamAsync(Stream input, Stream output, byte[] key, byte[] iv, CancellationToken ct = default);
    }
}
