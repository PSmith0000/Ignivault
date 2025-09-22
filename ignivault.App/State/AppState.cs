namespace ignivault.App.State
{
    public class AppState
    {
        public byte[]? KeySalt { get; private set; }

        public string UserEmail { get; set; } = string.Empty;

        public void SetUserState(string? base64Salt)
        {
            KeySalt = string.IsNullOrEmpty(base64Salt) ? null : Convert.FromBase64String(base64Salt);
        }

        public void ClearUserState()
        {
            UserEmail = string.Empty;
            KeySalt = null;
        }
    }
}
