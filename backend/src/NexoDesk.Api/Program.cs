using Microsoft.EntityFrameworkCore;
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