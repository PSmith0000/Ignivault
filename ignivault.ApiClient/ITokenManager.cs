namespace ignivault.ApiClient
{
    public interface ITokenManager
    {
        Task<string?> GetTokenAsync();
        Task SetTokenAsync(string? token);
        Task ClearTokenAsync();
    }
}
