[ApiController]
[Route(ApiEndpoints.Account.Base)]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IUserActivityRepository _activityRepository;

    public AccountController(IAccountService accountService, IUserActivityRepository activityRepository)
    {
        _accountService = accountService;
        _activityRepository = activityRepository;
    }


    /// <summary>
    /// Gets the profile information of the currently authenticated user.
    /// </summary>
    /// <returns></returns>
    [HttpGet(ApiEndpoints.Account.Profile)]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetCurrentUserId();
        var profile = await _accountService.GetProfileAsync(userId);
        if (profile == null)
        {
            return NotFound();
        }
        return Ok(profile);
    }

    /// <summary>
    /// Updates the user's login password after verifying their current password.
    /// </summary>
    /// <param name="passwordDto"></param>
    /// <returns></returns>
    [HttpPut(ApiEndpoints.Account.Password)]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequestDto passwordDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetCurrentUserId();
        var result = await _accountService.UpdatePasswordAsync(userId, passwordDto);

        if (!result.Succeeded)
        {
            return BadRequest(new { Message = "Password update failed.", Errors = result.Errors });
        }
        return NoContent(); // Success
    }


    /// <summary>
    /// Updates the user's master password after verifying their current password and receiving re-encrypted vault items.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut(ApiEndpoints.Account.MasterPassword)]
    public async Task<IActionResult> UpdateMasterPassword([FromBody] UpdateMasterPasswordRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetCurrentUserId();
        var result = await _accountService.UpdateMasterPasswordAsync(userId, request);

        if (!result.Succeeded)
        {
            return BadRequest(new { Message = "Master password update failed.", Errors = result.Errors });
        }

        return NoContent(); 
    }

    /// <summary>
    /// Gets recent user activities for the current user.
    /// </summary>
    /// <param name="limit"></param>
    /// <returns></returns>
    [HttpGet(ApiEndpoints.Account.Activity)]
    public async Task<IActionResult> GetRecentActivity([FromQuery] int limit = 10)
    {
        var userId = GetCurrentUserId();
        var activities = await _activityRepository.GetRecentActivitiesForUserAsync(userId, limit);

        var activityDtos = activities.Select(a => new UserActivityDto
        {
            ActivityType = a.ActivityType.ToString(),
            Timestamp = a.Timestamp,
            Success = a.Success,
            IpAddress = a.IpAddress,
            Details = a.Details
        });

        return Ok(activityDtos);
    }

    /// <summary>
    /// Disables two-factor authentication for the current user.
    /// </summary>
    /// <returns></returns>
    [HttpPost(ApiEndpoints.Account.Disable2fa)]
    public async Task<IActionResult> Disable2Fa()
    {
        var userId = GetCurrentUserId();
        var result = await _accountService.Disable2FaAsync(userId);
        if (!result.Succeeded)
        {
            return BadRequest(new { Message = "Failed to disable 2FA.", Errors = result.Errors });
        }
        return Ok(new { Message = "2FA has been disabled successfully." });
    }


    /// <summary>
    /// Regenerates new recovery codes for the current user.
    /// </summary>
    /// <returns></returns>
    [HttpPost(ApiEndpoints.Account.RegenerateRecoveryCodes)]
    public async Task<IActionResult> RegenerateRecoveryCodes()
    {
        var userId = GetCurrentUserId();
        var codes = await _accountService.RegenerateRecoveryCodesAsync(userId);
        if (codes == null)
        {
            return BadRequest(new { Message = "Could not regenerate codes. Ensure 2FA is enabled." });
        }
        return Ok(codes);
    }

    /// <summary>
    /// Gets the current user's ID from the JWT token.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private string GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            throw new InvalidOperationException("User ID not found in token.");
        }
        return userId;
    }
}