using System;
using System.Collections.Generic;
using System.Text;

namespace ignivault.Shared
{
    public class ApiEndpoints
    {
        private const string ApiBase = "api";

        /// <summary>
        /// Authentication and user management endpoints.
        /// </summary>
        public static class Auth
        {
            public const string Base = $"{ApiBase}/auth";
            public const string Login = "login";
            public const string Login2fa = "login-2fa";
            public const string Register = "register";
            public const string Enable2fa = "enable-2fa";
            public const string Verify2fa = "verify-2fa";
            public const string ForgotPassword = "forgot-password";
            public const string ResetPassword = "reset-password";

            public const string LoginEndpoint = $"{Base}/{Login}";
            public const string Login2faEndpoint = $"{Base}/{Login2fa}";
            public const string RegisterEndpoint = $"{Base}/{Register}";
            public const string Enable2faEndpoint = $"{Base}/{Enable2fa}";
            public const string Verify2faEndpoint = $"{Base}/{Verify2fa}";
            public const string ForgotPasswordEndpoint = $"{Base}/{ForgotPassword}";
            public const string ResetPasswordEndpoint = $"{Base}/{ResetPassword}";
        }

        /// <summary>
        /// Account management endpoints for profile, password, 2FA, and activity.
        /// </summary>
        public static class Account
        {
            public const string Base = $"{ApiBase}/account";
            public const string Profile = "profile";
            public const string Password = "password";
            public const string MasterPassword = "master-password";
            public const string Activity = "activity";
            public const string Disable2fa = "disable-2fa";
            public const string RegenerateRecoveryCodes = "regenerate-codes";

            public const string ProfileEndpoint = $"{Base}/{Profile}";
            public const string PasswordEndpoint = $"{Base}/{Password}";
            public const string MasterPasswordEndpoint = $"{Base}/{MasterPassword}";
            public const string ActivityEndpoint = $"{Base}/{Activity}";
            public const string Disable2faEndpoint = $"{Base}/{Disable2fa}";
            public const string RegenerateRecoveryCodesEndpoint = $"{Base}/{RegenerateRecoveryCodes}";
        }

        /// <summary>
        /// Vault management endpoints for items and file handling.
        /// </summary>
        public static class Vault
        {
            public const string Base = $"{ApiBase}/vault";
            public const string Items = "items";
            public const string ItemById = "items/{id}";
            public const string Files = "files";
            public const string DownloadFile = "files/{id}/download";

            public const string ItemsEndpoint = $"{Base}/{Items}";
            public const string ItemByIdEndpoint = $"{Base}/{ItemById}";
            public const string FilesEndpoint = $"{Base}/{Files}";
            public const string DownloadFileEndpoint = $"{Base}/{DownloadFile}";
        }

        /// <summary>
        /// Administrative endpoints for user and role management.
        /// </summary>
        public static class Admin
        {
            private const string ApiBase = "api";
            public const string Base = $"{ApiBase}/admin";

            public const string Users = $"{Base}/users";
            public const string Roles = $"{Base}/roles";

            public const string UserRoles = $"{Users}/{{userId}}/roles";
            public const string LockUser = $"{Users}/{{userId}}/lock";
            public const string UnlockUser = $"{Users}/{{userId}}/unlock";
        }
    }
}
