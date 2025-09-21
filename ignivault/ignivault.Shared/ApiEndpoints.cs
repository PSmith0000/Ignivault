using System;
using System.Collections.Generic;
using System.Text;

namespace ignivault.Shared
{
    public class ApiEndpoints
    {
        private const string ApiBase = "api";

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
    }
}
