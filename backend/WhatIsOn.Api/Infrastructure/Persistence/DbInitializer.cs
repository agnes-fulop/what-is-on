using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WhatIsOn.Application.Auth.Interfaces;
using WhatIsOn.Domain.Entities;
using WhatIsOn.Domain.Enums;
using WhatIsOn.Domain.ValueObjects;

namespace WhatIsOn.Infrastructure.Persistence;

/// <summary>
/// Development-time seed. Inserts a small representative graph (one organizer,
/// one regular user, one VIP user, one speaker, one non-VIP event, one VIP
/// event with a nested layout exercising every component type) so the read
/// endpoints can be verified end-to-end without manual data entry. Idempotent —
/// no-ops if events already exist.
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

        var passwordHash = passwordHasher.Hash(DemoPassword);

        var organizer = new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Email = "organizer@example.com",
            DisplayName = "Demo Organizer",
            PasswordHash = passwordHash,
            Role = UserRole.Organizer,
            CreatedAt = DateTime.UtcNow
        };

        var regular = new User
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Email = "regular@example.com",
            DisplayName = "Regular User",
            PasswordHash = passwordHash,
            Role = UserRole.Regular,
            CreatedAt = DateTime.UtcNow
        };

        var vip = new User
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Email = "vip@example.com",
            DisplayName = "VIP User",
            PasswordHash = passwordHash,
            Role = UserRole.Vip,
            CreatedAt = DateTime.UtcNow
        };

        var speaker = new Speaker
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            Name = "Dr. Jane Doe",
            Title = "VP of Engineering, Example Corp",
            Bio = "Expert in serverless architectures and cloud-native development.",
            Image = "https://example.com/jane.jpg"
        };

        var publicEvent = BuildPublicEvent(organizer.Id, speaker.Id);
        var (vipEvent, layout) = BuildVipEvent(organizer.Id, speaker.Id);

        context.Users.AddRange(organizer, regular, vip);
        context.Speakers.Add(speaker);
        context.Layouts.Add(layout);
        context.Events.AddRange(publicEvent, vipEvent);

        await context.SaveChangesAsync(cancellationToken);
    }

    private static Event BuildPublicEvent(Guid organizerId, Guid speakerId)
    {
        var eventId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        return new Event
        {
            Id = eventId,
            Title = "Cloud Innovators Summit 2026",
            Subtitle = "Explore the future of cloud computing",
            Description = "A one-day event focused on cloud-native development.",
            IsVip = false,
            Date = new DateOnly(2026, 10, 26),
            Hero = new Hero { Image = "https://example.com/hero.jpg", CtaText = "Register Now" },
            Location = new Location { City = "Las Vegas", Venue = "The Venetian", Address = "3355 Las Vegas Blvd S" },
            Registration = new RegistrationInfo
            {
                OpenDate = new DateOnly(2026, 8, 1),
                CloseDate = new DateOnly(2026, 10, 20),
                Fee = 99m,
                EarlyBirdDiscount = 20m
            },
            OrganizerId = organizerId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Sessions = new List<Session>
            {
                new()
                {
                    Id = Guid.Parse("aaaaaaaa-1111-1111-1111-111111111111"),
                    Title = "Building Serverless Applications",
                    Description = "How to build scalable serverless apps.",
                    From = new DateTime(2026, 10, 26, 10, 0, 0, DateTimeKind.Utc),
                    To = new DateTime(2026, 10, 26, 11, 0, 0, DateTimeKind.Utc),
                    Level = SessionLevel.Intermediate,
                    Track = "Serverless",
                    Room = "Ballroom A",
                    SortOrder = 0,
                    SpeakerId = speakerId
                }
            }
        };
    }

    private static (Event Event, Layout Layout) BuildVipEvent(Guid organizerId, Guid speakerId)
    {
        var eventId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var sessionId = Guid.Parse("bbbbbbbb-1111-1111-1111-111111111111");
        var layoutId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        var session = new Session
        {
            Id = sessionId,
            Title = "VIP Roadmap Briefing",
            Description = "Closed-door look at next year's platform.",
            From = new DateTime(2026, 11, 15, 14, 0, 0, DateTimeKind.Utc),
            To = new DateTime(2026, 11, 15, 15, 0, 0, DateTimeKind.Utc),
            Level = SessionLevel.Advanced,
            Track = "Strategy",
            Room = "Boardroom",
            SortOrder = 0,
            SpeakerId = speakerId
        };

        var layout = BuildVipLayout(layoutId, speakerId, sessionId);

        var @event = new Event
        {
            Id = eventId,
            Title = "VIP Strategy Day 2026",
            Subtitle = "Members-only roadmap and networking",
            Description = "An intimate event for VIP members.",
            IsVip = true,
            Date = new DateOnly(2026, 11, 15),
            Hero = new Hero { Image = "https://example.com/vip-hero.jpg", CtaText = "Reserve Seat" },
            Location = new Location { City = "New York", Venue = "Private Club", Address = "1 Park Ave" },
            Registration = new RegistrationInfo
            {
                OpenDate = new DateOnly(2026, 9, 1),
                CloseDate = new DateOnly(2026, 11, 10),
                Fee = 499m,
                EarlyBirdDiscount = 0m
            },
            OrganizerId = organizerId,
            LayoutId = layoutId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Sessions = new List<Session> { session }
        };

        return (@event, layout);
    }

    /// <summary>
    /// Hand-built tree exercising every component type. SpeakerCard stores the
    /// speakerId reference; SessionCard stores the sessionId reference. The
    /// LayoutTreeBuilder enriches both at read time.
    /// </summary>
    private static Layout BuildVipLayout(Guid layoutId, Guid speakerId, Guid sessionId)
    {
        var rootSection = new LayoutComponent
        {
            Id = Guid.NewGuid(),
            LayoutId = layoutId,
            ComponentType = ComponentType.Section,
            Data = JsonSerializer.Serialize(new { direction = "col" }),
            SortOrder = 0
        };

        var heading = new LayoutComponent
        {
            Id = Guid.NewGuid(),
            LayoutId = layoutId,
            ParentComponentId = rootSection.Id,
            ComponentType = ComponentType.Heading,
            Data = JsonSerializer.Serialize(new { text = "About this Event", level = "h2" }),
            SortOrder = 0
        };

        var paragraph = new LayoutComponent
        {
            Id = Guid.NewGuid(),
            LayoutId = layoutId,
            ParentComponentId = rootSection.Id,
            ComponentType = ComponentType.Paragraph,
            Data = JsonSerializer.Serialize(new { text = "An exclusive day for VIP members." }),
            SortOrder = 1
        };

        var speakerList = new LayoutComponent
        {
            Id = Guid.NewGuid(),
            LayoutId = layoutId,
            ComponentType = ComponentType.SpeakerList,
            Data = JsonSerializer.Serialize(new { title = "Featured Speakers" }),
            SortOrder = 1
        };

        var speakerCard = new LayoutComponent
        {
            Id = Guid.NewGuid(),
            LayoutId = layoutId,
            ParentComponentId = speakerList.Id,
            ComponentType = ComponentType.SpeakerCard,
            Data = JsonSerializer.Serialize(new { speakerId }),
            SortOrder = 0
        };

        var sessionSchedule = new LayoutComponent
        {
            Id = Guid.NewGuid(),
            LayoutId = layoutId,
            ComponentType = ComponentType.SessionSchedule,
            Data = JsonSerializer.Serialize(new { title = "Schedule" }),
            SortOrder = 2
        };

        var sessionCard = new LayoutComponent
        {
            Id = Guid.NewGuid(),
            LayoutId = layoutId,
            ParentComponentId = sessionSchedule.Id,
            ComponentType = ComponentType.SessionCard,
            Data = JsonSerializer.Serialize(new { sessionId }),
            SortOrder = 0
        };

        return new Layout
        {
            Id = layoutId,
            Name = "VIP Layout",
            Components = new List<LayoutComponent>
            {
                rootSection, heading, paragraph,
                speakerList, speakerCard,
                sessionSchedule, sessionCard
            }
        };
    }
}
