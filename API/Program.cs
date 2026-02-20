using Microsoft.EntityFrameworkCore;
using WebApi_PocV1.Infrastructure.Database;
using WebApi_PocV1.Repositories.Implementations;
using WebApi_PocV1.Repositories.Interfaces;
using WebApi_PocV1.Services.Implementations;
using WebApi_PocV1.Services.Interfaces;
using WebApi_PocV1.Storage;
using WebApi_PocV1.Storage.Implementations;

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
                "http://localhost:5031",                    // wa local
                "https://p3api-acc.gielvangaal.dev",        // api acc
                "https://p3api-prod.gielvangaal.dev",       // api prod
                "https://cine.net-acc.gielvangaal.dev",     // wa acc
                "https://cine.net-prod.gielvangaal.dev"     // wa prod
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