namespace ignivault.WebAPI.Data.Enums
{
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
