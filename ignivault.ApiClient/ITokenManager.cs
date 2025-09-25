namespace ignivault.ApiClient
{
    public interface ITokenManager
    {
        /// <summary>
        /// Gets the stored token, or null if no token is stored.
        /// </summary>
        /// <returns></returns>
        Task<string?> GetTokenAsync();

        /// <summary>
        /// Sets the token to be stored. If null is passed, it clears the stored token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SetTokenAsync(string? token);

        /// <summary>
        /// Clears the stored token.
        /// </summary>
        /// <returns></returns>
        Task ClearTokenAsync();
    }
}
