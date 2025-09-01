using ignivault.API.Models.Records;
using ignivault.API.Security.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ignivault.API.SQL
{
    public class AppDbContext : IdentityDbContext<LoginUser, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<VaultItem> VaultItems { get; set; }
    }
}
