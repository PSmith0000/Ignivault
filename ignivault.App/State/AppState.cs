namespace ignivault.App.State
{
    public class AppState
    {
        public byte[]? KeySalt { get; private set; }

        public string UserEmail { get; set; } = string.Empty;


        /// <summary>
        /// Sets the user-specific state, including email and key salt.
        /// </summary>
        /// <param name="base64Salt"></param>
        public void SetUserState(string? base64Salt)
        {
            KeySalt = string.IsNullOrEmpty(base64Salt) ? null : Convert.FromBase64String(base64Salt);
        }

        /// <summary>
        /// Clears the user-specific state, including email and key salt.
        /// </summary>
        public void ClearUserState()
        {
            UserEmail = string.Empty;
            KeySalt = null;
        }
    }
}
