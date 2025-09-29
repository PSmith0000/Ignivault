using ignivault.WebAPI.Data.Entities;
using ignivault.WebAPI.Extensions;
using ignivault.WebAPI.Middleware;
using Microsoft.AspNetCore.Identity;

// ============================================================
// Application Entry Point
// Configures and runs the Ignivault Web API.
// ============================================================

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// --- Configure Kestrel Server ---
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 104_857_600; // 100 MB
});

// --- Configure Logging ---
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

// --- Register Services for Dependency Injection ---

// Registers the custom global exception handler.
builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddProblemDetails();

// Registers controllers and services for API documentation (Swagger).
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registers all custom application services using extension methods for cleanliness.
builder.Services.AddDatabaseServices(configuration);
builder.Services.AddIdentityAndAuthentication(configuration);
builder.Services.AddCorsPolicies(configuration);
builder.Services.AddRepositoryServices();
builder.Services.AddApplicationServices();

var app = builder.Build();

// --- Initial Database Data ---
await SeedRolesAsync(app.Services);
await CreateDefaultAdminsAsync(app.Services);


// ============================================================
// Configure the HTTP Request Pipeline
// ============================================================

// Use the registered global exception handler.
app.UseExceptionHandler();

// Enable Swagger UI only in the development environment.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// Apply the configured CORS policy.
app.UseCors("AllowClient");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


//Ensures that the default "Admin" and "User" roles exist in the database.
static async Task SeedRolesAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

//Creates default administrator accounts based on the configuration in app-settings.json.
static async Task CreateDefaultAdminsAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<LoginUser>>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var adminUsers = configuration.GetSection("AdminUsers").Get<List<AdminUserConfig>>();

    if (adminUsers == null || !adminUsers.Any())
    {
        Console.WriteLine("No admin users configured.");
        return;
    }

    Console.WriteLine("Creating Default Admin Accounts...");

    foreach (var adminConfig in adminUsers)
    {
        if (string.IsNullOrEmpty(adminConfig.Email) || string.IsNullOrEmpty(adminConfig.Password)) continue;

        if (await userManager.FindByEmailAsync(adminConfig.Email) == null)
        {
            var salt = System.Security.Cryptography.RandomNumberGenerator.GetBytes(16);

            var adminUser = new LoginUser
            {
                UserName = adminConfig.Email,
                Email = adminConfig.Email,
                EmailConfirmed = true,
                KeySalt = salt
            };

            var result = await userManager.CreateAsync(adminUser, adminConfig.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                await userManager.AddToRoleAsync(adminUser, "User"); // Also add User role
                Console.WriteLine($"Admin User '{adminConfig.Email}' created successfully.");
            }
        }
    }
}

/// <summary>
/// Helper class to bind the admin user configuration from app-settings.json.
/// </summary>
internal class AdminUserConfig
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}