using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReferralProgram.Servercore.Data;
using ReferralProgram.Servercore.Services;

namespace ReferralProgram.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ReferralDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Remove existing service registrations
            RemoveService<IReferralService>(services);
            RemoveService<ISmsService>(services);

            // Create and open an in-memory SQLite connection
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // Add DbContext using in-memory SQLite
            services.AddDbContext<ReferralDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            // Register test services
            services.AddScoped<ISmsService, MockSmsService>();
            services.AddScoped<IReferralService, SqliteReferralService>();

            // Build service provider and create database
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ReferralDbContext>();
            db.Database.EnsureCreated();
        });
    }

    private static void RemoveService<T>(IServiceCollection services)
    {
        var descriptors = services.Where(d => d.ServiceType == typeof(T)).ToList();
        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection?.Dispose();
        }
    }
}
