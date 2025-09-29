namespace ignivault.WebAPI.Services
{
    public interface IAccountService
    {
        /// <summary>
        /// Retrieves the non-sensitive profile information for a user.
        /// </summary>
        /// <param name="userId">The ID of the user whose profile is being requested.</param>
        /// <returns>A DTO with the user's profile information, or null if the user is not found.</returns>
        Task<UserProfileDto?> GetProfileAsync(string userId);

        /// <summary>
        /// Updates the user's login password after verifying their old password.
        /// </summary>
        /// <param name="userId">The ID of the user changing their password.</param>
        /// <param name="passwordDto">The DTO containing the old and new passwords.</param>
        /// <returns>An `IdentityResult` indicating the outcome of the operation.</returns>
        Task<IdentityResult> UpdatePasswordAsync(string userId, UpdatePasswordRequestDto passwordDto);

        /// <summary>
        /// Securely updates the user's master password after receiving a payload of re-encrypted vault items from the client.
        /// </summary>
        /// <param name="userId">The ID of the user changing their master password.</param>
        /// <param name="request">The DTO containing the verification password, new salt, and re-encrypted items.</param>
        /// <returns>An `IdentityResult` indicating the outcome of the operation.</returns>
        Task<IdentityResult> UpdateMasterPasswordAsync(string userId, UpdateMasterPasswordRequestDto request);

        /// <summary>
        /// Disables Two-Factor Authentication for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>An `IdentityResult` indicating the outcome of the operation.</returns>
        Task<IdentityResult> Disable2FaAsync(string userId);

        /// <summary>
        /// Generates a new set of 2FA recovery codes for a user, invalidating any old ones.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A collection of new recovery codes, or null if 2FA is not enabled for the user.</returns>
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

        public async Task<IdentityResult> Disable2FaAsync(string userId)
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