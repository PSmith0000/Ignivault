using ignivault.Shared;
using ignivault.Shared.DTOs;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ignivault.ApiClient.Admin
{
    public interface IAdminApiClient
    {
        /// <summary>
        /// Gets a list of all users.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<UserDto>>> GetUsersAsync();

        /// <summary>
        /// Locks a user account, preventing them from logging in.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ApiResponse> LockUserAsync(string userId);

        /// <summary>
        /// Unlocks a user account, allowing them to log in again.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ApiResponse> UnlockUserAsync(string userId);

        /// <summary>
        /// Gets a list of all available roles.
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<string>>> GetRolesAsync();

        /// <summary>
        /// Gets the roles assigned to a specific user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<string>>> GetUserRolesAsync(string userId);

        /// <summary>
        /// Adds a role to a specific user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<ApiResponse> AddRoleToUserAsync(string userId, string roleName);

        /// <summary>
        /// Removes a role from a specific user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        Task<ApiResponse> RemoveRoleFromUserAsync(string userId, string roleName);
    }

    public class AdminApiClient : IAdminApiClient
    {
        private readonly HttpClient _httpClient;
        public AdminApiClient(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<ApiResponse<IEnumerable<UserDto>>> GetUsersAsync()
        {
            var users = await _httpClient.GetFromJsonAsync<IEnumerable<UserDto>>(ApiEndpoints.Admin.Users);
            return new ApiResponse<IEnumerable<UserDto>> { IsSuccess = true, Data = users };
        }

        public async Task<ApiResponse> LockUserAsync(string userId)
        {
            var url = ApiEndpoints.Admin.LockUser.Replace("{userId}", userId);
            var response = await _httpClient.PostAsync(url, null);
            return new ApiResponse { IsSuccess = response.IsSuccessStatusCode };
        }

        public async Task<ApiResponse> UnlockUserAsync(string userId)
        {
            var url = ApiEndpoints.Admin.UnlockUser.Replace("{userId}", userId);
            var response = await _httpClient.PostAsync(url, null);
            return new ApiResponse { IsSuccess = response.IsSuccessStatusCode };
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetRolesAsync()
        {
            // FIX: Deserialize the simple array of strings that the API is sending.
            var roles = await _httpClient.GetFromJsonAsync<IEnumerable<string>>(ApiEndpoints.Admin.Roles);
            // Then, wrap the result in your ApiResponse object.
            return new ApiResponse<IEnumerable<string>> { IsSuccess = true, Data = roles };
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetUserRolesAsync(string userId)
        {
            var url = ApiEndpoints.Admin.UserRoles.Replace("{userId}", userId);
            // FIX: Deserialize the simple array of strings that the API is sending.
            var roles = await _httpClient.GetFromJsonAsync<IEnumerable<string>>(url);
            return new ApiResponse<IEnumerable<string>> { IsSuccess = true, Data = roles };
        }

        public async Task<ApiResponse> AddRoleToUserAsync(string userId, string roleName)
        {
            var url = ApiEndpoints.Admin.UserRoles.Replace("{userId}", userId);
            var response = await _httpClient.PostAsJsonAsync(url, new RoleRequestDto { RoleName = roleName });
            return new ApiResponse { IsSuccess = response.IsSuccessStatusCode };
        }

        public async Task<ApiResponse> RemoveRoleFromUserAsync(string userId, string roleName)
        {
            var url = ApiEndpoints.Admin.UserRoles.Replace("{userId}", userId);
            var request = new HttpRequestMessage(HttpMethod.Delete, url)
            {
                Content = JsonContent.Create(new RoleRequestDto { RoleName = roleName })
            };
            var response = await _httpClient.SendAsync(request);
            return new ApiResponse { IsSuccess = response.IsSuccessStatusCode };
        }
    }
}