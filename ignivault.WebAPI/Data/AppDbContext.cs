namespace ignivault.WebAPI.Data
{
    public class AppDbContext : IdentityDbContext<LoginUser, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<VaultEntity> VaultItems { get; set; }
        public DbSet<StoredBlob> StoredBlobs { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }

        /// <summary>
        /// OnModelCreating is used to configure the EF Core model.
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<VaultEntity>()
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<VaultEntity>()
                .HasOne(v => v.StoredBlob)
                .WithOne(b => b.VaultItem) 
                .HasForeignKey<StoredBlob>(b => b.VaultItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserActivity>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserActivity>()
                .HasIndex(a => a.Timestamp);
        }
    }
}