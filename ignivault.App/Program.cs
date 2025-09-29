using Blazored.SessionStorage;
using ignivault.ApiClient;
using ignivault.ApiClient.Account;
using ignivault.ApiClient.Admin;
using ignivault.ApiClient.Admin.Reports;
using ignivault.ApiClient.Auth;
using ignivault.ApiClient.Vault;
using ignivault.App;
using ignivault.App.Services;
using ignivault.App.State;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Charts.Chart.Internal;
using Syncfusion.Licensing;
using System.Net.Http;
var builder = WebAssemblyHostBuilder.CreateDefault(args);
//Get required configuration properties
var _webApiBase = builder.Configuration["Http:ApiBase"];




builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//Register Syncfusion and Blazored Session Storage
builder.Services.AddSyncfusionBlazor();
builder.Services.AddBlazoredSessionStorage();

//Register Blazors core authorization services
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("IsAdmin", policy => policy.RequireRole("Admin"));
});
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

//Register more auth stuff
builder.Services.AddScoped<ITokenManager, TokenManager>();
builder.Services.AddScoped<AuthHeaderHandler>();


//Configure HttpClient
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(_webApiBase ?? "NULL");
    client.Timeout = TimeSpan.FromMinutes(5);
}).AddHttpMessageHandler<AuthHeaderHandler>();
builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>(client =>
{
    client.BaseAddress = new Uri(_webApiBase!);
    client.Timeout = TimeSpan.FromMinutes(5);
}).AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddHttpClient<IAccountApiClient, AccountApiClient>(client =>
{
    client.BaseAddress = new Uri(_webApiBase!);
    client.Timeout = TimeSpan.FromMinutes(5);
}).AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddHttpClient<IVaultApiClient, VaultApiClient>(client =>
{
    client.BaseAddress = new Uri(_webApiBase!);
    client.Timeout = TimeSpan.FromMinutes(10);
}).AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddHttpClient<IAdminApiClient, AdminApiClient>(client =>
{
    client.BaseAddress = new Uri(_webApiBase!);
    client.Timeout = TimeSpan.FromMinutes(5);
}).AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddHttpClient<IReportsApiClient, ReportsApiClient>(client =>
{
    client.BaseAddress = new Uri(_webApiBase!);
    client.Timeout = TimeSpan.FromMinutes(5);
}).AddHttpMessageHandler<AuthHeaderHandler>();

//Register all the API clients
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient"));
builder.Services.AddScoped<IAuthApiClient, AuthApiClient>();
builder.Services.AddScoped<IAccountApiClient, AccountApiClient>();
builder.Services.AddScoped<IVaultApiClient, VaultApiClient>();
builder.Services.AddScoped<IReportsApiClient, ReportsApiClient>();
builder.Services.AddScoped<IAdminApiClient, AdminApiClient>();

//Register Client side services
builder.Services.AddSingleton<AppState>();
builder.Services.AddScoped<ICryptoService, CryptoService>();

await builder.Build().RunAsync();