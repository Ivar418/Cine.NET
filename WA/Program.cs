using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Services;
using WA;
using WA.Auth;
using WA.Services;
using WA.Services.Http;
using WA.Services.Http.Interfaces;

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

// "https://p3api-acc.gielvangaal.dev/"
string url;
var hostEnvironment = builder.HostEnvironment;

if (hostEnvironment.IsDevelopment())
{
    url = "http://localhost:8080/";
}
else if (hostEnvironment.IsStaging())
{
    url = "https://p3api-acc.gielvangaal.dev/";
}
else if (hostEnvironment.IsProduction())
{
    url = "https://p3api-prod.gielvangaal.dev/";
}
else
{
    url = "https://p3api-prod.gielvangaal.dev/";
}

// HTTP
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri($"{url}") });

// WA API/SERVICES
builder.Services.AddScoped<IUserApi, UserApi>();
builder.Services.AddScoped<LayoutStateService>();

await builder.Build().RunAsync();