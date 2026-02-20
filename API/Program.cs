using API.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using API.Infrastructure.Database;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using API.Services.Implementations;
using API.Services.Interfaces;
using API.Storage;
using API.Storage.Implementations;

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
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPhotoStorage, LocalPhotoStorage>();

// Monitoring: health check endpoint
builder.Services.AddHealthChecks();

// ORM: configure EF Core with MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(cs, ServerVersion.AutoDetect(cs));
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorWasm", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5031",              // dev
                "https://p3api-acc.gielvangaal.dev",     // acc
                "https://p3api-prod.gielvangaal.dev"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// App build: finalize configuration
var app = builder.Build();

// Network: configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Security: redirect HTTP to HTTPS
app.UseHttpsRedirection();

app.UseStaticFiles(); // serves wwwroot/*
    
// Routing: enable routing
app.UseRouting();

// Security: CORS
app.UseCors("BlazorWasm");

// Security: authorization middleware
app.UseAuthorization();

// Monitoring: expose /health
app.MapHealthChecks("/health");

// Routing: map controller endpoints
app.MapControllers();

// Runtime: start web application
app.Run();

// # Test