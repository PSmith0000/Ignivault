using Blazored.LocalStorage;
using ignivault;
using ignivault.Core.Interface;
using ignivault.Data;
using ignivault.Layout;
using ignivault.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.JSInterop;
using Syncfusion.Blazor;
using Syncfusion.XlsIO.Implementation;
using System.Globalization;

try
{
    //Register Syncfusion license https://help.syncfusion.com/common/essential-studio/licensing/how-to-generate
    Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXdcdXZURmVdUUV0X0VWYEk=");
    var builder = WebAssemblyHostBuilder.CreateDefault(args);



    builder.Services.AddMemoryCache();
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");
    builder.Services.AddSyncfusionBlazor();
    builder.Services.AddSingleton(typeof(ISyncfusionStringLocalizer), typeof(SyncfusionLocalizer));

    // Set the default culture of the application
    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
    CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

    // Get the modified culture from culture switcher
    var host = builder.Build();
    var jsInterop = host.Services.GetRequiredService<IJSRuntime>();
    var result = await jsInterop.InvokeAsync<string>("cultureInfo.get");
    if (result != null)
    {
        // Set the culture from culture switcher
        var culture = new CultureInfo(result);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    builder.Services.AddScoped(sp =>
        new HttpClient { BaseAddress = new Uri("https://localhost:7158/") });
    builder.Services.AddSingleton<VaultService>();
    builder.Services.AddBlazoredLocalStorage();
    builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
    builder.Services.AddScoped<AuthService>();
    builder.Services.AddScoped<IHttpService, HttpService>();
    builder.Services.AddScoped<AccountService>();
    builder.Services.AddAuthorizationCore();

    var app = builder.Build();

    await app.RunAsync();
}
catch
{
    Console.WriteLine("WebApp Failed To Load: ");
}
