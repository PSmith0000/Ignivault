using Microsoft.AspNetCore.Http.Timeouts;

[ApiController]
[Route(ApiEndpoints.Vault.Base)]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class VaultController : ControllerBase
{
    private readonly IVaultService _vaultService;
    private readonly IFileService _fileService;

    public VaultController(IVaultService vaultService, IFileService fileService)
    {
        _vaultService = vaultService;
        _fileService = fileService;
    }


    /// <summary>
    /// Gets a summary list of all vault items for the current user.
    /// </summary>
    /// <returns></returns>
    [HttpGet(ApiEndpoints.Vault.Items)]
    public async Task<IActionResult> GetItems()
    {
        var userId = GetCurrentUserId();
        var items = await _vaultService.GetItemsAsync(userId);
        return Ok(items);
    }


    /// <summary>
    /// Gets a single, detailed vault item by its ID for the current user.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet(ApiEndpoints.Vault.ItemById)]
    public async Task<IActionResult> GetItem(int id)
    {
        var userId = GetCurrentUserId();
        var item = await _vaultService.GetItemAsync(id, userId);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    /// <summary>
    /// Creates a new vault item with a pre-encrypted payload.
    /// </summary>
    /// <param name="itemDto"></param>
    /// <returns></returns>
    [HttpPost(ApiEndpoints.Vault.Items)]
    public async Task<IActionResult> CreateItem([FromBody] CreateVaultItemDto itemDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var userId = GetCurrentUserId();
        var newItem = await _vaultService.CreateItemAsync(itemDto, userId);
        return CreatedAtAction(nameof(GetItem), new { id = newItem.Id }, newItem);
    }


    /// <summary>
    /// Updates an existing vault item's name and optionally its encrypted data.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="itemDto"></param>
    /// <returns></returns>
    [HttpPut(ApiEndpoints.Vault.ItemById)]
    public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateVaultItemDto itemDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var userId = GetCurrentUserId();
        var success = await _vaultService.UpdateItemAsync(id, itemDto, userId);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }


    /// <summary>
    /// Deletes a vault item by its ID for the current user.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete(ApiEndpoints.Vault.ItemById)]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var userId = GetCurrentUserId();
        var success = await _vaultService.DeleteItemAsync(id, userId);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }


    /// <summary>
    /// Uploads a file to be stored as a vault item. The file must be pre-encrypted client-side.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost(ApiEndpoints.Vault.Files)]
    [RequestSizeLimit(104_857_600)]
    [RequestTimeout(600000)]
    public async Task<IActionResult> UploadFile([FromForm] FileUploadRequestDto request)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest(new { Message = "No file was provided." });
        }
        if (string.IsNullOrEmpty(request.IvBase64))
        {
            return BadRequest(new { Message = "The Initialization Vector (IV) is missing." });
        }

        var userId = GetCurrentUserId();
        var iv = Convert.FromBase64String(request.IvBase64);
        await using var stream = request.File.OpenReadStream();

        var newFileItem = await _fileService.CreateFileAsync(request.File.FileName, stream, iv, userId);
        return CreatedAtAction(nameof(GetItem), new { id = newFileItem.Id }, newFileItem);
    }

    /// <summary>
    /// Downloads a file vault item by its ID for the current user.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>

    [HttpGet(ApiEndpoints.Vault.DownloadFile)]
    [RequestTimeout(600000)]
    public async Task<IActionResult> DownloadFile(int id)
    {
        var userId = GetCurrentUserId();
        var fileDto = await _fileService.GetFileForDownloadAsync(id, userId);
        if (fileDto == null)
        {
            return NotFound("Content not found.");
        }

        return File(fileDto.EncryptedData, fileDto.ContentType, fileDto.FileName);
    }


    /// <summary>
    /// Gets the current authenticated user's ID from the JWT claims.
    /// </summary>
    /// <returns></returns>
    private string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }
}