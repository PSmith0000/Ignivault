using ignivault.Data.Models.Auth;

namespace ignivault.Services
{
    public class AccountService
    {
        public LoginUser LoginUser { get; set; }

        public void SetAccount(LoginUser user)
        {
           LoginUser = user;
        }
    }
}
