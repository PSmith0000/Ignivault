namespace ignivault.WebAPI.Services
{
    public interface IAccountService
    {
        Task<UserProfileDto?> GetProfileAsync(string userId);

        Task<IdentityResult> UpdatePasswordAsync(string userId, UpdatePasswordRequestDto passwordDto);

        Task<IdentityResult> UpdateMasterPasswordAsync(string userId, UpdateMasterPasswordRequestDto request);

        Task<IdentityResult> Disable2faAsync(string userId);
        Task<IEnumerable<string>?> RegenerateRecoveryCodesAsync(string userId);
    }

    public class AccountService : IAccountService
    {
        private readonly UserManager<LoginUser> _userManager;
        private readonly IVaultItemRepository _vaultItemRepository;
        private readonly AppDbContext _context;
        private readonly ILogger<AccountService> _logger;
        private readonly IUserActivityLogger _userActivityLogger;

        public AccountService(UserManager<LoginUser> userManager, IVaultItemRepository vaultItemRepository, AppDbContext context, ILogger<AccountService> logger, IUserActivityLogger userActivityLogger)
        {
            _userManager = userManager;
            _vaultItemRepository = vaultItemRepository;
            _context = context;
            _logger = logger;
            _userActivityLogger = userActivityLogger;
        }

        public async Task<UserProfileDto?> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            return new UserProfileDto
            {
                Username = user.UserName,
                Email = user.Email,
                Is2faEnabled = user.TwoFactorEnabled
            };
        }

        public async Task<IdentityResult> Disable2faAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            return await _userManager.SetTwoFactorEnabledAsync(user, false);
        }

        public async Task<IEnumerable<string>?> RegenerateRecoveryCodesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.TwoFactorEnabled) return null;

            return await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        }

        public async Task<IdentityResult> UpdatePasswordAsync(string userId, UpdatePasswordRequestDto passwordDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(user, passwordDto.OldPassword, passwordDto.NewPassword);

            if (result.Succeeded)
            {
                await _userActivityLogger.LogActivityAsync(userId, ActivityType.PasswordChanged, true, "Password changed successfully.");
            }

            return result;
        }

        public async Task<IdentityResult> UpdateMasterPasswordAsync(string userId, UpdateMasterPasswordRequestDto request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
            if (!isPasswordCorrect)
            {
                return IdentityResult.Failed(new IdentityError { Code = "PasswordMismatch", Description = "Incorrect password." });
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                user.KeySalt = request.NewSalt;
                var identityResult = await _userManager.UpdateAsync(user);
                if (!identityResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return identityResult;
                }

                foreach (var reEncryptedItem in request.ReEncryptedItems)
                {
                    var vaultItem = await _vaultItemRepository.GetItemByIdAndUserIdAsync(reEncryptedItem.ItemId, userId);
                    if (vaultItem != null)
                    {
                        vaultItem.EncryptedData = reEncryptedItem.EncryptedData;
                        vaultItem.Iv = reEncryptedItem.Iv;
                        vaultItem.UpdatedAt = DateTime.UtcNow;
                        await _vaultItemRepository.UpdateItemAsync(vaultItem);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Vault item with ID {reEncryptedItem.ItemId} not found for user.");
                    }
                }

                await transaction.CommitAsync();
                await _userActivityLogger.LogActivityAsync(userId, ActivityType.MasterPasswordChanged, true, "Master password changed and items re-encrypted.");
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update master password for user {UserId}", userId);
                await transaction.RollbackAsync();
                return IdentityResult.Failed(new IdentityError { Description = "An unexpected error occurred. The operation was rolled back." });
            }
        }
    }
}