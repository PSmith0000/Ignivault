using ignivault.API.Security.Auth;
using ignivault.API.Services;
using ignivault.API.SQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace ignivault.API
{
    public class Program
    {
        public static void Main(string[] args)
        {


            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = 450L * 1024 * 1024; // 450 MB
            });

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
            

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //SQL Server Settings
            builder.Services.AddDbContext<AppDbContext>(options => 
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<LoginUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            

            builder.Services.AddScoped<UserActivityService>();
            builder.Services.AddTransient<IEmailService, EmailService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowBlazor", policy =>
                {
                    policy.WithOrigins(new string[] { "https://localhost:7085", "https://localhost:7185", "http://localhost:65453" })
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            //Authentication Settings
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

            builder.Services.AddAuthorization();

            //security stuff

            //General stuff
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = 10 * 1024 * 1024;
            });

            var app = builder.Build();

            //addtional auth
            AddRolesAsync(app.Services).Wait();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors("AllowBlazor");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        static async Task AddRolesAsync(IServiceProvider services)
        {
            using var scope = services.CreateAsyncScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Admin", "User"};

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}