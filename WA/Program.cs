using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Cine.NET_WA;
using Cine.NET_WA.Api;
using Cine.NET_WA.Auth;
using Cine.NET_WA.Repositories;
using Cine.NET_WA.Services;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using WA;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri("https://p3api-prod.gielvangaal.dev/") });

builder.Services.AddScoped<IUserApi, UserApi>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

await builder.Build().RunAsync();
