using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WhatIsOn.Application.Auth.Interfaces;
using WhatIsOn.Infrastructure.Persistence.Seed;

namespace WhatIsOn.Infrastructure.Persistence;

/// <summary>
/// Development-time seed orchestrator. Applies pending migrations, then —
/// if the database is empty — inserts the canonical demo catalog from the
/// <see cref="Seed"/> sub-folder. Idempotent: a second run is a no-op.
/// </summary>
public static class DbInitializer
{
    private const string DemoPassword = "demo-password-123";

    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<WhatIsOnDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await context.Database.MigrateAsync(cancellationToken);

        if (await context.Events.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTime.UtcNow;
        var passwordHash = passwordHasher.Hash(DemoPassword);

        // Insert order matters: speakers and users before events (FK targets),
        // layouts before events (events reference LayoutId), events last.
        context.Users.AddRange(UsersSeed.Build(passwordHash, now));
        context.Speakers.AddRange(SpeakersSeed.Build());
        context.Layouts.AddRange(LayoutsSeed.Build());
        context.Events.AddRange(EventsSeed.Build(now));

        await context.SaveChangesAsync(cancellationToken);
    }
}
