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
    Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXdcdXZURmVdUUV0X0VWYEk=");

    var builder = WebAssemblyHostBuilder.CreateDefault(args);

    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");

    // Http Client
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7158/") });

    // Blazored LocalStorage
    builder.Services.AddBlazoredSessionStorage();

    // Authentication & Vault Services
    builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
    builder.Services.AddScoped<AuthService>();
    builder.Services.AddScoped<AccountService>();
    builder.Services.AddScoped<VaultService>();
    builder.Services.AddScoped<IHttpService, HttpService>();

    builder.Services.AddAuthorizationCore();

    // Syncfusion
    builder.Services.AddSyncfusionBlazor();

    // Culture Setup
    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
    CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

    var host = builder.Build();
    var jsInterop = host.Services.GetRequiredService<IJSRuntime>();

    var cultureName = await jsInterop.InvokeAsync<string>("cultureInfo.get");
    if (!string.IsNullOrEmpty(cultureName))
    {
        var culture = new CultureInfo(cultureName);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    await host.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"WebAssembly host failed to load: {ex.Message}");
}
