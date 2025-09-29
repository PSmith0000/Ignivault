namespace ignivault.WebAPI.Data.Entities
{
    /// <summary>
    /// LoginUser extends the IdentityUser class to include additional properties specific to the application's user management needs.
    /// </summary>
    public class LoginUser : IdentityUser
    {
        [Required]
        public byte[] KeySalt { get; set; }
    }
}
