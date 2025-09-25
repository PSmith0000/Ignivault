namespace ignivault.WebAPI.Data.Enums
{
    /// <summary>
    /// ActivityType enumerates the various types of user and system activities that can be logged for auditing and monitoring purposes.
    /// </summary>
    public enum ActivityType
    {
        UserLogin,
        UserLoginFailed,
        UserLogout,
        UserRegistered,

        VaultItemCreated,
        VaultItemViewed,
        VaultItemUpdated,
        VaultItemDeleted,
        FileDownloaded,

        PasswordChanged,
        MasterPasswordChanged,
        TwoFactorEnabled,
        TwoFactorDisabled
    }
}
