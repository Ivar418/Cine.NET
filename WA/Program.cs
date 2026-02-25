using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using WA;
using WA.ApiClients;
using WA.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri("https://p3api-prod.gielvangaal.dev/") });
    // new HttpClient { BaseAddress = new Uri("https://p3api-acc.gielvangaal.dev/") });
    // new HttpClient { BaseAddress = new Uri("http://localhost:8080/") });

builder.Services.AddScoped<IUserApi, UserApi>();

await builder.Build().RunAsync();
