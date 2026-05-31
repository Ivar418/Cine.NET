using System.Text;
using API.Domain.Model;
using API.Infrastructure.Database;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using API.Services.Background.Implementations;
using API.Services.Implementations;
using API.Services.Interfaces;
using API.src.Repositories.Implementations;
using API.Storage.Implementations;
using API.Storage.Interfaces;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

Env.Load();
// App setup: create builder + dependency container
var builder = WebApplication.CreateBuilder(args);

// MVC: enable controllers and routing
builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// API docs: enable Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI: register application services
/*
 * Transient: A new instance is provided every time the service is requested.
 * Scoped: A single instance is provided per request.
 * Singleton: A single instance is created and shared throughout the application's lifetime.
 */

//Users
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
//Movies
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();
//Tickets
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
builder.Services.AddScoped<ITicketTypeService, TicketTypeService>();
//Photos
builder.Services.AddScoped<IPhotoStorage, LocalPhotoStorage>();

//Seat reservations
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReservationService, ReservationService>();
//Showings
builder.Services.AddScoped<IShowingRepository, ShowingRepository>();
builder.Services.AddScoped<IShowingService, ShowingService>();
//Auditoriums
builder.Services.AddScoped<IAuditoriumRepository, AuditoriumRepository>();
builder.Services.AddScoped<IAuditoriumService, AuditoriumService>();
//Pricings
builder.Services.AddScoped<IPricingConfigRepository, PricingConfigRepository>();
builder.Services.AddScoped<IPricingService, PricingService>();
//Orders
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderPdfService, OrderPdfService>();
// TicketRule
builder.Services.AddScoped<ITicketRuleService, TicketRuleService>();

// Emails
builder.Services.AddScoped<IMailSubscriptionRepository, MailSubscriptionRepository>();
builder.Services.AddScoped<ILocalMailService, LocalMailService>();
// Arrangement
builder.Services.AddScoped<IArrangementService, ArrangementService>();
builder.Services.AddScoped<IArrangementRepository, ArrangementRepository>();

// Authentication
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();


// Get JWT variables or set them from .env
builder.Configuration["JwtSettings:Key"] ??=
    Environment.GetEnvironmentVariable("JWT_KEY");

builder.Configuration["JwtSettings:Issuer"] ??=
    Environment.GetEnvironmentVariable("JWT_ISSUER");

builder.Configuration["JwtSettings:Audience"] ??=
    Environment.GetEnvironmentVariable("JWT_AUDIENCE");

builder.Configuration["JwtSettings:ExpiryMinutes"] ??=
    Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES");

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

// Background jobs: extend mock showings once per day
builder.Services.AddHostedService<DailyShowingsGeneratorService>();


// Monitoring: health check endpoint
builder.Services.AddHealthChecks();

// ORM: configure EF Core with MySQL
builder.Services.AddDbContextPool<ApiDbContext>(options => {
    // Try to get connection string from environment/Docker
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrEmpty(connectionString)) {
        var database = Environment.GetEnvironmentVariable("DB_NAME") ?? "my_local_db";
        var user = Environment.GetEnvironmentVariable("DB_USER") ?? "root";
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "secret";

        // Fallback for local debugging
        Console.WriteLine("DefaultConnection not found in environment. Using local MySQL connection.");
        connectionString = $"Server=localhost;Port=3306;Database={database};User={user};Password={password};";
    }
    else {
        Console.WriteLine("Using DefaultConnection from environment.");
    }

    // Wait for MySQL if needed (optional for local debugging, you can skip retries locally)
    ServerVersion? serverVersion = null;
    var retries = 0;
    const int maxRetries = 10;
    var delay = TimeSpan.FromSeconds(5);

    while (serverVersion == null && retries < maxRetries) {
        try {
            serverVersion = ServerVersion.AutoDetect(connectionString);
        }
        catch (MySqlConnector.MySqlException) {
            retries++;
            Console.WriteLine($"MySQL not ready yet. Retry {retries}/{maxRetries} in {delay.TotalSeconds} seconds...");
            Thread.Sleep(delay);
        }
    }

    if (serverVersion == null)
        throw new InvalidOperationException("Could not connect to MySQL to detect server version.");

    options.UseMySql(
        connectionString,
        serverVersion,
        mySqlOptions => {
            mySqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null
            );
        }
    );
});


// JWT configuration
var jwtSettings = builder.Configuration
                      .GetSection("JwtSettings")
                      .Get<JwtSettings>()
                  ?? throw new Exception("JwtSettings missing");
// Validate if the key is up to JWT requirements
jwtSettings.Validate();


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key)),

            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero
        };
    });

// CORS
var allowedOrigins = new List<string>();
if (builder.Environment.IsProduction()) {
    allowedOrigins.Add("https://prod-cinenetwa.ivarvisser.nl");
}
else {
    allowedOrigins.AddRange(new[] {
        "https://acc-cinenetwa.ivarvisser.nl",
        "http://localhost:5031", // Blazor WASM local dev
        "http://localhost:8082" // KotlinMP local dev
    });
}

builder.Services.AddCors(options => {
    options.AddPolicy("BlazorWasm", policy => {
        policy
            .WithOrigins(allowedOrigins.ToArray())
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// App build: finalize configuration
var app = builder.Build();

// Network: configure HTTP request pipeline
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Security: redirect HTTP to HTTPS
if (!app.Environment.IsDevelopment()) {
    app.UseHttpsRedirection();
}

app.UseStaticFiles(); // serves wwwroot/*

// Routing: enable routing
app.UseRouting();

// Security: CORS
app.UseCors("BlazorWasm");

// Add jwt
app.UseAuthentication();

// Security: authorization middleware
app.UseAuthorization();

// Monitoring: expose /health
app.MapHealthChecks("/health");

// Routing: map controller endpoints
app.MapControllers();

// Database: apply pending migrations at startup and seed some mock data
_ = Task.Run(async () => {
    using var scope = app.Services.CreateScope();

    var services = scope.ServiceProvider;

    var db = services.GetRequiredService<ApiDbContext>();

    var movieService = services.GetRequiredService<IMovieService>();
    var showingService = services.GetRequiredService<IShowingService>();
    var ticketService = services.GetRequiredService<ITicketService>();
    var pricingService = services.GetRequiredService<IPricingService>();
    var auditoriumService = services.GetRequiredService<IAuditoriumService>();
    var mailService = services.GetRequiredService<ILocalMailService>();
    var authService = services.GetRequiredService<IAuthService>();

    try {
        await db.Database.EnsureCreatedAsync();

        await DbSeeder.SeedAsync(
            db,
            movieService,
            showingService,
            ticketService,
            pricingService,
            auditoriumService,
            mailService,
            authService
        );

        Console.WriteLine("Database seeding completed.");
    }
    catch (Exception ex) {
        Console.WriteLine("Seeding produced an error: " + ex);
    }
});

// Runtime: start web application
app.Run();

// # Test