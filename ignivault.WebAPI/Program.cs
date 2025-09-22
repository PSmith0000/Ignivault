using ignivault.WebAPI.Data.Entities;
using ignivault.WebAPI.Extensions;
using ignivault.WebAPI.Middleware;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 104_857_600; // 100 MB
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---Extension Methods for Service Registration ---
builder.Services.AddDatabaseServices(configuration);
builder.Services.AddIdentityAndAuthentication(configuration);
builder.Services.AddCorsPolicies(configuration);
builder.Services.AddRepositoryServices();
builder.Services.AddApplicationServices();
var app = builder.Build();

// --- Roles ---
await SeedRolesAsync(app.Services);

// --- Create Default Admin Users ---
await CreateDefaultAdminsAsync(app.Services);

// --- Middleware Pipeline ---

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors("AllowClient");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();



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
                Console.WriteLine($"Admin User '{adminConfig.Email}' created successfully.");
            }
        }
    }
}

class AdminUserConfig
{
    public string Email { get; set; }
    public string Password { get; set; }
}