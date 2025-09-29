using Microsoft.JSInterop;
using System.Security.Cryptography;
using System.Text.Json;

namespace ignivault.App.Services
{

    public record EncryptionResult(byte[] EncryptedData, byte[] Iv);

    public interface ICryptoService
    {
        Task<string?> DecryptFileAndGetBase64Async(byte[] encryptedData, byte[] iv, byte[] salt, string masterPassword);

        Task<EncryptionResult?> EncryptAsync<T>(T data, byte[] salt, string masterPassword);
        Task<T?> DecryptAsync<T>(byte[] encryptedData, byte[] iv, byte[] salt, string masterPassword);

        Task<EncryptionResult?> EncryptFileAsync(byte[] fileBytes, byte[] salt, string masterPassword);
        Task<byte[]?> DecryptFileAsync(byte[] encryptedData, byte[] iv, byte[] salt, string masterPassword);
    }

    public class CryptoService : ICryptoService
    {
        private readonly IJSRuntime _jsRuntime;

        public CryptoService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<T?> DecryptAsync<T>(byte[] encryptedData, byte[] iv, byte[] salt, string masterPassword)
        {
            var decryptedBytes = await _jsRuntime.InvokeAsync<byte[]?>("decryptData", encryptedData, iv, salt, masterPassword);
            if (decryptedBytes == null)
            {
                throw new CryptographicException("Decryption failed. The master password may be incorrect.");
            }
            return JsonSerializer.Deserialize<T>(decryptedBytes, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<byte[]?> DecryptFileAsync(byte[] encryptedData, byte[] iv, byte[] salt, string masterPassword)
        {
            var decryptedBytes = await _jsRuntime.InvokeAsync<byte[]?>("decryptData", encryptedData, iv, salt, masterPassword);

            if (decryptedBytes == null)
            {
                throw new CryptographicException("File decryption failed. The master password may be incorrect.");
            }

            return decryptedBytes;
        }

        public async Task<string?> DecryptFileAndGetBase64Async(byte[] encryptedData, byte[] iv, byte[] salt, string masterPassword)
        {
            var decryptedDataBase64 = await _jsRuntime.InvokeAsync<string?>("decryptData", encryptedData, iv, salt, masterPassword);

            if (string.IsNullOrEmpty(decryptedDataBase64))
            {
                throw new CryptographicException("File decryption failed. The master password may be incorrect.");
            }

            return decryptedDataBase64;
        }

        public async Task<EncryptionResult?> EncryptAsync<T>(T data, byte[] salt, string masterPassword)
        {
            var dataBytes = JsonSerializer.SerializeToUtf8Bytes(data);
            return await EncryptBytesAsync(dataBytes, salt, masterPassword);
        }

        public async Task<EncryptionResult?> EncryptFileAsync(byte[] fileBytes, byte[] salt, string masterPassword)
        {
            return await EncryptBytesAsync(fileBytes, salt, masterPassword);
        }

        private async Task<EncryptionResult?> EncryptBytesAsync(byte[] dataBytes, byte[] salt, string masterPassword)
        {
            var iv = RandomNumberGenerator.GetBytes(16);
            var result = await _jsRuntime.InvokeAsync<JsonElement?>("encryptData", dataBytes, iv, salt, masterPassword);
            if (result == null) throw new CryptographicException("Encryption failed in JavaScript.");

            var encryptedDataBase64 = result.Value.GetProperty("encryptedData").GetString();
            var returnedIvBase64 = result.Value.GetProperty("iv").GetString();

            return new EncryptionResult(
                Convert.FromBase64String(encryptedDataBase64!),
                Convert.FromBase64String(returnedIvBase64!)
            );
        }
    }
}