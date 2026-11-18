using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using NexoDesk.Infrastructure.Identity;
using NexoDesk.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// OpenAPI
builder.Services.AddOpenApi();

// Database connection
var connectionString = builder.Configuration
    .GetConnectionString("NexoDesk")
    ?? throw new InvalidOperationException(
        "Connection string 'NexoDesk' was not found. Configure it using User Secrets.");

builder.Services.AddDbContext<NexoDeskDbContext>(options =>
    options.UseSqlServer(connectionString));

// ASP.NET Core Data Protection
builder.Services.AddDataProtection();

// ASP.NET Core Identity
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        // User settings
        options.User.RequireUniqueEmail = true;

        // Password settings
        options.Password.RequiredLength = 10;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;

        // Lockout settings
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);

        // Email confirmation
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<NexoDeskDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();