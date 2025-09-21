using Blazored.SessionStorage;
using ignivault.ApiClient;
using ignivault.ApiClient.Account;
using ignivault.ApiClient.Auth;
using ignivault.ApiClient.Vault;
using ignivault.App;
using ignivault.App.Services;
using ignivault.App.State;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;
using Syncfusion.Licensing;
using System.Net.Http;
var builder = WebAssemblyHostBuilder.CreateDefault(args);
//Get required configuration properties
var _webApiBase = builder.Configuration["Http:ApiBase"];
var _syncfusionKey = builder.Configuration["Syncfusion:ApiKey"];
SyncfusionLicenseProvider.RegisterLicense(_syncfusionKey);




builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//Register Syncfusion and Blazored Session Storage
builder.Services.AddSyncfusionBlazor();
builder.Services.AddBlazoredSessionStorage();

//Register Blazors core authorization services
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

//Register the token manager and the AuthHandler
builder.Services.AddScoped<ITokenManager, TokenManager>();
builder.Services.AddScoped<AuthHeaderHandler>();


//Configure HttpClient
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(_webApiBase ?? "NULL");
}).AddHttpMessageHandler<AuthHeaderHandler>();

//Register all the strongly-typed API clients
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient"));
builder.Services.AddScoped<IAuthApiClient, AuthApiClient>();
builder.Services.AddScoped<IAccountApiClient, AccountApiClient>();
builder.Services.AddScoped<IVaultApiClient, VaultApiClient>();

//Register Client-side services
builder.Services.AddSingleton<AppState>();
builder.Services.AddScoped<ICryptoService, CryptoService>();

await builder.Build().RunAsync();