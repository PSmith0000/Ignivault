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

    [HttpPost(ApiEndpoints.Account.Disable2fa)]
    public async Task<IActionResult> Disable2fa()
    {
        var userId = GetCurrentUserId();
        var result = await _accountService.Disable2faAsync(userId);
        if (!result.Succeeded)
        {
            return BadRequest(new { Message = "Failed to disable 2FA.", Errors = result.Errors });
        }
        return Ok(new { Message = "2FA has been disabled successfully." });
    }

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