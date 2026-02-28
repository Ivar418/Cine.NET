using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Services;
using WA;
using WA.ApiClients;
using WA.Auth;
using WA.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// MUDBLAZOR
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
    config.SnackbarConfiguration.MaxDisplayedSnackbars = 3;
    config.SnackbarConfiguration.PreventDuplicates = true;
});

// AUTH
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

// HTTP
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri("https://p3api-prod.gielvangaal.dev/") });
    // new HttpClient { BaseAddress = new Uri("https://p3api-acc.gielvangaal.dev/") });
    // new HttpClient { BaseAddress = new Uri("http://localhost:8080/") });

// WA API/SERVICES
builder.Services.AddScoped<IUserApi, UserApi>();
builder.Services.AddScoped<LayoutStateService>();

await builder.Build().RunAsync();
