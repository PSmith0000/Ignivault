namespace ignivault.WebAPI.Data.Entities
{
    public class LoginUser : IdentityUser
    {
        [Required]
        public byte[] KeySalt { get; set; }
    }
}
