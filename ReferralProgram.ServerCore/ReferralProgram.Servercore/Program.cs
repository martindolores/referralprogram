using Microsoft.AspNetCore.RateLimiting;
using ReferralProgram.Servercore.Services;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add CORS for React frontend (Netlify)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "http://localhost:5173",
            "https://*.netlify.app"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// Add Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1)
            }));
    
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Register services (using mock implementations for now)
builder.Services.AddSingleton<ISmsService, MockSmsService>();
builder.Services.AddSingleton<IReferralService, MockReferralService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();
