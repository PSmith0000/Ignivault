using ignivault.Shared;
using ignivault.Shared.DTOs;
using ignivault.WebAPI.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ignivault.WebAPI.Controllers
{
    [Route(ApiEndpoints.Admin.Base)]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<LoginUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<LoginUser> _signInManager;   

        public AdminController(UserManager<LoginUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<LoginUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }


        /// <summary>
        /// Gets a list of all users with their basic information.
        /// </summary>
        /// <returns></returns>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.UserName,
                    Email = u.Email,
                    LockoutEnd = u.LockoutEnd
                })
                .ToListAsync();
            return Ok(users);
        }

        /// <summary>
        /// Gets a list of all available roles.
        /// </summary>
        /// <returns></returns>
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return Ok(roles);
        }

        /// <summary>
        /// Gets the roles assigned to a specific user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("users/{userId}/roles")]
        public async Task<IActionResult> GetUserRoles([FromRoute] string userId) // FIX 5: Added [FromRoute]
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found.");

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(roles);
        }


        /// <summary>
        /// Adds a role to a specific user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("users/{userId}/roles")]
        public async Task<IActionResult> AddRoleToUser([FromRoute] string userId, [FromBody] RoleRequestDto request) // FIX 5: Added [FromRoute]
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found.");

            if (!await _roleManager.RoleExistsAsync(request.RoleName)) return BadRequest("Role does not exist.");

            var result = await _userManager.AddToRoleAsync(user, request.RoleName);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok();
        }

        /// <summary>
        /// Removes a role from a specific user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete("users/{userId}/roles")]
        public async Task<IActionResult> RemoveRoleFromUser([FromRoute] string userId, [FromBody] RoleRequestDto request) // FIX 5: Added [FromRoute]
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found.");

            if (!await _roleManager.RoleExistsAsync(request.RoleName)) return BadRequest("Role does not exist.");

            var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok();
        }


        /// <summary>
        /// Locks a user account, preventing them from logging in.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("users/{userId}/lock")]
        public async Task<IActionResult> LockUser([FromRoute] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            if(_signInManager.IsSignedIn(User))
            {
                await _signInManager.SignOutAsync();
            }

            return Ok(new { Message = $"User {user.UserName} has been locked." });
        }


        /// <summary>
        /// Unlocks a user account, allowing them to log in again.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("users/{userId}/unlock")]
        public async Task<IActionResult> UnlockUser([FromRoute] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, null);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { Message = $"User {user.UserName} has been unlocked." });
        }
    }
}