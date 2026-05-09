using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WhatIsOn.Infrastructure.Persistence;

namespace WhatIsOn.Tests.Integration;

/// <summary>
/// Hosts the API in-process for integration tests using a long-lived SQLite
/// :memory: connection, so each factory instance gets a clean isolated DB
/// (the dev seed runs once on startup). Jwt settings are supplied via
/// in-memory configuration so tests don't depend on user-secrets.
/// </summary>
public class WhatIsOnWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public WhatIsOnWebApplicationFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "WhatIsOn.Tests",
                ["Jwt:Audience"] = "WhatIsOn.Tests",
                ["Jwt:Key"] = "test-key-with-at-least-thirty-two-bytes-of-entropy-aaaa",
                ["Jwt:ExpirationMinutes"] = "60",
            });
        });

        builder.ConfigureServices(services =>
        {
            // Replace the DbContext registration with one bound to our in-memory connection.
            var dbDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<WhatIsOnDbContext>));
            if (dbDescriptor is not null) services.Remove(dbDescriptor);

            services.AddDbContext<WhatIsOnDbContext>(options => options.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection.Dispose();
        }
        base.Dispose(disposing);
    }
}
