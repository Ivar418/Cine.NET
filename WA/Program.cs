using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Services;
using WA;
using WA.Auth;
using WA.Services;
using WA.Services.Http;
using WA.Services.Http.Implementation;
using WA.Services.Http.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//LOCALIZATION
builder.Services.AddLocalization();

// MUDBLAZOR
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
    config.SnackbarConfiguration.MaxDisplayedSnackbars = 3;
    config.SnackbarConfiguration.PreventDuplicates = true;
});
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddJsonFile("appsettings.Staging.json", optional: true)
    .AddJsonFile("appsettings.Production.json", optional: true);

// AUTH
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

var apiUrl = builder.Configuration["ApiUrl"];
Console.WriteLine($"API URL: {apiUrl}");
Console.WriteLine(builder.HostEnvironment.Environment);

// HTTP
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(apiUrl) });

// WA API/SERVICES
builder.Services.AddScoped<IUserApi, UserApi>();
builder.Services.AddScoped<LayoutStateService>();
builder.Services.AddScoped<IMovieApiClient, MovieApiClient>();
builder.Services.AddScoped<IShowingApi, ShowingApi>();
builder.Services.AddScoped<IAuditoriumApi, AuditoriumApi>();
builder.Services.AddScoped<ISeatFinderApiClient, SeatFinderApiService>();
builder.Services.AddScoped<IArrangementApi, ArrangementApi>();
builder.Services.AddScoped<IOrderApi, OrderApi>();

// Mail related
builder.Services.AddScoped<IMailApi, MailApi>();

var host = builder.Build();

// Standaard cultuur instellen
var culture = new CultureInfo("nl");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await host.RunAsync();