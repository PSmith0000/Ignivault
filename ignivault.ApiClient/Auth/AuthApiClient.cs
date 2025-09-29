using System.Net.NetworkInformation;

namespace ignivault.ApiClient.Auth
{
    public interface IAuthApiClient
    {
        /// <summary>
        /// Occurs when a user successfully logs in.
        /// </summary>
        event Func<Task>? OnLoginSuccess;

        /// <summary>
        /// Occurs when a user logs out.
        /// </summary>
        event Func<Task>? OnLogout;

        /// <summary>
        /// Registers a new user with the provided registration details.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ApiResponse> RegisterAsync(RegisterRequestDto request);

        /// <summary>
        /// Logs in a user with the provided credentials.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ApiResponse<LoginResponseDto>> LoginAsync(ignivault.Shared.DTOs.Auth.LoginRequestDto request);

        /// <summary>
        /// Logs in a user with 2FA code after initial login indicates 2FA is required.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ApiResponse<LoginResponseDto>> Login2faAsync(Login2faRequestDto request);

        /// <summary>
        /// Forgot password - initiates the password reset process by sending a reset email.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordRequestDto request);

        /// <summary>
        /// Resets the user's password using the token and new password provided.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequestDto request);

        /// <summary>
        /// Enables two-factor authentication for the current user, returning the setup details.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<Enable2faResponseDto>> Enable2faAsync();

        /// <summary>
        /// Verifies the 2FA code provided by the user to complete the 2FA setup process.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ApiResponse<Verify2faResponseDto>> Verify2faAsync(Verify2faRequestDto request);

        /// <summary>
        /// Logs out the current user by clearing the stored authentication token.
        /// </summary>
        /// <returns></returns>
        Task LogoutAsync();
    }
    public class AuthApiClient : IAuthApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenManager _tokenManager;

        public event Func<Task>? OnLoginSuccess;
        public event Func<Task>? OnLogout;

        public AuthApiClient(HttpClient httpClient, ITokenManager tokenManager)
        {
            _httpClient = httpClient;
            _tokenManager = tokenManager;
        }

        // --- Core Auth ---

        public async Task<ApiResponse> RegisterAsync(RegisterRequestDto request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Auth.RegisterEndpoint, request);
            if (response.IsSuccessStatusCode) return new ApiResponse { IsSuccess = true };
            return await CreateErrorResponseAsync(response);
        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(ignivault.Shared.DTOs.Auth.LoginRequestDto request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Auth.LoginEndpoint, request);
            if (!response.IsSuccessStatusCode) return await CreateErrorResponseAsync<LoginResponseDto>(response);

            var loginResult = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            if (loginResult == null) return new ApiResponse<LoginResponseDto> { IsSuccess = false, Message = "Invalid server response." };

            if (!loginResult.Is2faRequired && loginResult.LoginResponse != null)
            {
                await _tokenManager.SetTokenAsync(loginResult.LoginResponse.Token);
                
                if (OnLoginSuccess != null) await OnLoginSuccess.Invoke();
            }

            return new ApiResponse<LoginResponseDto> { IsSuccess = true, Data = loginResult };
        }

        public async Task LogoutAsync()
        {
            await _tokenManager.ClearTokenAsync();
            if (OnLogout != null) await OnLogout.Invoke();
        }

        // --- 2FA Flow ---

        public async Task<ApiResponse<LoginResponseDto>> Login2faAsync(Login2faRequestDto request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Auth.Login2faEndpoint, request);
            if (!response.IsSuccessStatusCode) return await CreateErrorResponseAsync<LoginResponseDto>(response);

            var payload = await response.Content.ReadFromJsonAsync<LoginResponsePayload>();
            if (payload == null) return new ApiResponse<LoginResponseDto> { IsSuccess = false, Message = "Invalid server response." };

            await _tokenManager.SetTokenAsync(payload.Token);
            if (OnLoginSuccess != null) await OnLoginSuccess.Invoke();

            var finalResult = new LoginResponseDto { Is2faRequired = false, LoginResponse = payload };
            return new ApiResponse<LoginResponseDto> { IsSuccess = true, Data = finalResult };
        }

        public async Task<ApiResponse<Enable2faResponseDto>> Enable2faAsync()
        {
            var response = await _httpClient.GetAsync(ApiEndpoints.Auth.Enable2faEndpoint);
            if (!response.IsSuccessStatusCode) return await CreateErrorResponseAsync<Enable2faResponseDto>(response);

            var data = await response.Content.ReadFromJsonAsync<Enable2faResponseDto>();
            return new ApiResponse<Enable2faResponseDto> { IsSuccess = true, Data = data };
        }

        public async Task<ApiResponse<Verify2faResponseDto>> Verify2faAsync(Verify2faRequestDto request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Auth.Verify2faEndpoint, request);
            if (!response.IsSuccessStatusCode) return await CreateErrorResponseAsync<Verify2faResponseDto>(response);

            var data = await response.Content.ReadFromJsonAsync<Verify2faResponseDto>();
            return new ApiResponse<Verify2faResponseDto> { IsSuccess = true, Data = data };
        }

        // --- Password Reset ---

        public async Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Auth.ForgotPasswordEndpoint, request);
            if (response.IsSuccessStatusCode) return new ApiResponse { IsSuccess = true };
            return await CreateErrorResponseAsync(response);
        }

        public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Auth.ResetPasswordEndpoint, request);
            if (response.IsSuccessStatusCode) return new ApiResponse { IsSuccess = true };
            return await CreateErrorResponseAsync(response);
        }

        private async Task<ApiResponse> CreateErrorResponseAsync(HttpResponseMessage response)
        {
            try
            {
                var errorDto = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
                var errorMessage = errorDto?.Errors?.FirstOrDefault() ?? errorDto?.Message ?? "An unknown error occurred.";
                return new ApiResponse { IsSuccess = false, Message = errorMessage };
            }
            catch
            {
                return new ApiResponse { IsSuccess = false, Message = "An unknown error occurred." };
            }
        }

        private async Task<ApiResponse<T>> CreateErrorResponseAsync<T>(HttpResponseMessage response)
        {
            try
            {
                var errorDto = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
                var errorMessage = errorDto?.Errors?.FirstOrDefault() ?? errorDto?.Message ?? "An unknown error occurred.";
                return new ApiResponse<T> { IsSuccess = false, Message = errorMessage };
            }
            catch
            {
                return new ApiResponse<T> { IsSuccess = false, Message = "An unknown error occurred." };
            }
        }
    }
}