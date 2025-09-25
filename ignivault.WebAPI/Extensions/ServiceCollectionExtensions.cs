namespace ignivault.WebAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Adds and configures the database context using SQL Server.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }

        /// <summary>
        /// Adds and configures Identity and JWT authentication services.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityAndAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<LoginUser, IdentityRole>(options =>
            {
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(configuration.GetValue<int>("LockoutSettings:LockoutDurationInMinutes"));
                options.Lockout.MaxFailedAccessAttempts = configuration.GetValue<int>("LockoutSettings:MaxFailedAccessAttempts");
                options.Lockout.AllowedForNewUsers = configuration.GetValue<bool>("LockoutSettings:AllowedForNewUsers");

            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(opts =>
            {
                opts.TokenLifespan = TimeSpan.FromHours(1);
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdmin", policy => policy.RequireRole("Admin"));
            });
            return services;
        }

        /// <summary>
        /// Adds CORS policies to allow requests from specified origins.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCorsPolicies(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowClient", policy =>
                {
                    policy.WithOrigins(allowedOrigins!)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            return services;
        }

        /// <summary>
        /// Configures all repository services for the application.
        /// </summary>
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IVaultItemRepository, VaultItemRepository>();
            services.AddScoped<IStoredBlobRepository, BlobRepository>();
            services.AddScoped<IUserActivityRepository, UserActivityRepository>();
            return services;
        }

        /// <summary>
        /// Configures all application specific services.
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IVaultService, VaultService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IUserActivityLogger, UserActivityLogger>();
            services.AddScoped<IEmailService, SendGridEmailService>();
            services.AddScoped<IReportsService, ReportsService>();
            services.AddHttpContextAccessor();

            return services;
        }
    }
}