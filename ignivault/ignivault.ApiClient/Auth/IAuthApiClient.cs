using System;
using System.Collections.Generic;
using System.Linq;
namespace ignivault.ApiClient.Auth
{
    public interface IAuthApiClient
    {
        event Func<Task>? OnLoginSuccess;
        event Func<Task>? OnLogout;

        Task<ApiResponse> RegisterAsync(RegisterRequestDto request);
        Task<ApiResponse<LoginResponseDto>> LoginAsync(ignivault.Shared.DTOs.Auth.LoginRequestDto request);
        Task<ApiResponse<LoginResponseDto>> Login2faAsync(Login2faRequestDto request);
        Task<ApiResponse> ForgotPasswordAsync(ForgotPasswordRequestDto request);
        Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequestDto request);
        Task<ApiResponse<Enable2faResponseDto>> Enable2faAsync();
        Task<ApiResponse<Verify2faResponseDto>> Verify2faAsync(Verify2faRequestDto request);
        Task LogoutAsync();
    }
}
