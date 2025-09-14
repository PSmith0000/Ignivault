using Blazored.SessionStorage;
using ignivault;
using ignivault.Core.Interface;
using ignivault.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using Syncfusion.Blazor;
using System.Globalization;

try
{
    Syncfusion.Licensing.SyncfusionLicenseProvider
        .RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXdcdXZURmVdUUV0X0VWYEk=");

    var builder = WebAssemblyHostBuilder.CreateDefault(args);

    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");

    // ---------- HTTP ----------
    builder.Services.AddScoped(sp =>
        new HttpClient { BaseAddress = new Uri("https://localhost:7158/") });

    // ---------- Storage & Auth ----------
    builder.Services.AddBlazoredSessionStorage();
    builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
    builder.Services.AddAuthorizationCore();

    // ---------- Application Services ----------
    builder.Services.AddScoped<AuthService>();
    builder.Services.AddScoped<AccountService>();
    builder.Services.AddScoped<VaultService>();
    builder.Services.AddScoped<IHttpService, HttpService>();

    // ---------- Syncfusion ----------
    builder.Services.AddSyncfusionBlazor();

    // ---------- Culture ----------
    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
    CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

    var host = builder.Build();

    // Restore user’s saved culture (optional)
    var js = host.Services.GetRequiredService<IJSRuntime>();
    var cultureName = await js.InvokeAsync<string>("cultureInfo.get");
    if (!string.IsNullOrEmpty(cultureName))
    {
        var culture = new CultureInfo(cultureName);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    var accountService = host.Services.GetRequiredService<AccountService>();
    await accountService.LoadUserAsync();

    await host.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"WebAssembly host failed to load: {ex.Message}");
}
