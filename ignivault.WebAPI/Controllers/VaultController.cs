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

    [HttpGet(ApiEndpoints.Vault.Items)]
    public async Task<IActionResult> GetItems()
    {
        var userId = GetCurrentUserId();
        var items = await _vaultService.GetItemsAsync(userId);
        return Ok(items);
    }

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

    [HttpPost(ApiEndpoints.Vault.Files)]
    [RequestSizeLimit(104_857_600)]
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

    [HttpGet(ApiEndpoints.Vault.DownloadFile)]
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

    private string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    }
}