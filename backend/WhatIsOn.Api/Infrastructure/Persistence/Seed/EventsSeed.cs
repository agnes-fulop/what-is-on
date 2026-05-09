using WhatIsOn.Domain.Entities;
using WhatIsOn.Domain.Enums;
using WhatIsOn.Domain.ValueObjects;

namespace WhatIsOn.Infrastructure.Persistence.Seed;

internal static class EventsSeed
{
    public static IReadOnlyList<Event> Build(DateTime createdAt)
    {
        return new[]
        {
            BuildAwsSummit(createdAt),
            BuildFrontendConference(createdAt),
            BuildVipRoundtable(createdAt),
            BuildPastSummit(createdAt)
        };
    }

    /// <summary>
    /// Public, currently registerable, has a rich layout exercising every
    /// component type — mirrors event.mock.json.
    /// </summary>
    private static Event BuildAwsSummit(DateTime createdAt) => new()
    {
        Id = SeedData.Events.AwsSummit,
        Title = "AWS Cloud Innovators Summit 2026",
        Subtitle = "Explore the future of cloud computing",
        Description = "A one-day summit covering serverless, ML, security, and DevRel best practices on AWS.",
        IsVip = false,
        Date = new DateOnly(2026, 10, 26),
        Hero = new Hero
        {
            Image = "https://example.com/events/aws/hero.jpg",
            CtaText = "Register Now"
        },
        Location = new Location
        {
            City = "Las Vegas",
            Venue = "The Venetian Resort",
            Address = "3355 Las Vegas Blvd S, Las Vegas, NV 89109"
        },
        Registration = new RegistrationInfo
        {
            OpenDate = new DateOnly(2026, 1, 1),
            CloseDate = new DateOnly(2026, 10, 20),
            Fee = 99m,
            EarlyBirdDiscount = 20m
        },
        OrganizerId = SeedData.Users.Organizer,
        LayoutId = SeedData.Layouts.AwsLayout,
        CreatedAt = createdAt,
        UpdatedAt = createdAt,
        Sessions = new List<Session>
        {
            new()
            {
                Id = SeedData.Sessions.AwsServerless,
                Title = "Building Serverless Applications with AWS Lambda",
                Description = "How to build scalable, cost-effective serverless apps using Lambda and friends.",
                From = new DateTime(2026, 10, 26, 10, 0, 0, DateTimeKind.Utc),
                To = new DateTime(2026, 10, 26, 11, 0, 0, DateTimeKind.Utc),
                Level = SessionLevel.Intermediate,
                Track = "Serverless",
                Room = "Venetian Ballroom A",
                SortOrder = 0,
                SpeakerId = SeedData.Speakers.JaneDoe
            },
            new()
            {
                Id = SeedData.Sessions.AwsMl,
                Title = "ML Inference at Scale",
                Description = "Lessons from running production ML on AWS infrastructure.",
                From = new DateTime(2026, 10, 26, 11, 30, 0, DateTimeKind.Utc),
                To = new DateTime(2026, 10, 26, 12, 30, 0, DateTimeKind.Utc),
                Level = SessionLevel.Advanced,
                Track = "AI/ML",
                Room = "Venetian Ballroom B",
                SortOrder = 1,
                SpeakerId = SeedData.Speakers.AishaKhan
            },
            new()
            {
                Id = SeedData.Sessions.AwsDevRel,
                Title = "Building Developer Communities",
                Description = "What works (and what doesn't) when growing a developer community.",
                From = new DateTime(2026, 10, 26, 14, 0, 0, DateTimeKind.Utc),
                To = new DateTime(2026, 10, 26, 14, 45, 0, DateTimeKind.Utc),
                Level = SessionLevel.Beginner,
                Track = "DevRel",
                Room = "Venetian Ballroom C",
                SortOrder = 2,
                SpeakerId = SeedData.Speakers.MarcusLee
            },
            new()
            {
                Id = SeedData.Sessions.AwsSecurity,
                Title = "Securing the Cloud Supply Chain",
                Description = "Threat modeling for cloud-native applications and dependencies.",
                From = new DateTime(2026, 10, 26, 15, 0, 0, DateTimeKind.Utc),
                To = new DateTime(2026, 10, 26, 16, 0, 0, DateTimeKind.Utc),
                Level = SessionLevel.Intermediate,
                Track = "Security",
                Room = "Venetian Ballroom A",
                SortOrder = 3,
                SpeakerId = SeedData.Speakers.ElenaCosta
            }
        }
    };

    /// <summary>
    /// Public, registration not yet open — exercises the registration-window gate.
    /// </summary>
    private static Event BuildFrontendConference(DateTime createdAt) => new()
    {
        Id = SeedData.Events.FrontendConf,
        Title = "Frontend Frameworks Conference 2026",
        Subtitle = "Three days of frontend deep-dives",
        Description = "Talks on React, SolidJS, web components, and the platform.",
        IsVip = false,
        Date = new DateOnly(2026, 11, 20),
        Hero = new Hero
        {
            Image = "https://example.com/events/frontend/hero.jpg",
            CtaText = "Save Your Seat"
        },
        Location = new Location
        {
            City = "Berlin",
            Venue = "TechHub Berlin",
            Address = "Karl-Liebknecht-Str 1, 10178 Berlin"
        },
        Registration = new RegistrationInfo
        {
            OpenDate = new DateOnly(2026, 8, 1),
            CloseDate = new DateOnly(2026, 11, 15),
            Fee = 149m,
            EarlyBirdDiscount = 30m
        },
        OrganizerId = SeedData.Users.Organizer,
        LayoutId = SeedData.Layouts.FrontendLayout,
        CreatedAt = createdAt,
        UpdatedAt = createdAt,
        Sessions = new List<Session>
        {
            new()
            {
                Id = SeedData.Sessions.FrontendReact,
                Title = "What's New in React 19",
                Description = "Tour of useActionState, useOptimistic, and the React Compiler.",
                From = new DateTime(2026, 11, 20, 9, 0, 0, DateTimeKind.Utc),
                To = new DateTime(2026, 11, 20, 10, 0, 0, DateTimeKind.Utc),
                Level = SessionLevel.Intermediate,
                Track = "React",
                Room = "Main Hall",
                SortOrder = 0,
                SpeakerId = SeedData.Speakers.MarcusLee
            },
            new()
            {
                Id = SeedData.Sessions.FrontendSolid,
                Title = "SolidJS in Production",
                Description = "Why one team migrated their dashboard from React to Solid.",
                From = new DateTime(2026, 11, 20, 10, 30, 0, DateTimeKind.Utc),
                To = new DateTime(2026, 11, 20, 11, 30, 0, DateTimeKind.Utc),
                Level = SessionLevel.Advanced,
                Track = "Frameworks",
                Room = "Hall B",
                SortOrder = 1,
                SpeakerId = null
            },
            new()
            {
                Id = SeedData.Sessions.FrontendWebComponents,
                Title = "Web Components in 2026",
                Description = "The state of declarative shadow DOM and form-associated custom elements.",
                From = new DateTime(2026, 11, 20, 13, 0, 0, DateTimeKind.Utc),
                To = new DateTime(2026, 11, 20, 14, 0, 0, DateTimeKind.Utc),
                Level = SessionLevel.Intermediate,
                Track = "Platform",
                Room = "Hall B",
                SortOrder = 2,
                SpeakerId = null
            }
        }
    };

    /// <summary>
    /// VIP-only, currently registerable, has a rich layout — exercises the VIP gate.
    /// </summary>
    private static Event BuildVipRoundtable(DateTime createdAt) => new()
    {
        Id = SeedData.Events.VipRoundtable,
        Title = "VIP Strategy Roundtable 2026",
        Subtitle = "Members-only roadmap and networking",
        Description = "An intimate day for VIP members. Closed-door briefings and structured networking.",
        IsVip = true,
        Date = new DateOnly(2026, 11, 15),
        Hero = new Hero
        {
            Image = "https://example.com/events/vip/hero.jpg",
            CtaText = "Reserve Seat"
        },
        Location = new Location
        {
            City = "New York",
            Venue = "Private Members' Club",
            Address = "1 Park Ave, New York, NY 10016"
        },
        Registration = new RegistrationInfo
        {
            OpenDate = new DateOnly(2026, 1, 1),
            CloseDate = new DateOnly(2026, 11, 10),
            Fee = 499m,
            EarlyBirdDiscount = 0m
        },
        OrganizerId = SeedData.Users.Organizer,
        LayoutId = SeedData.Layouts.VipLayout,
        CreatedAt = createdAt,
        UpdatedAt = createdAt,
        Sessions = new List<Session>
        {
            new()
            {
                Id = SeedData.Sessions.VipRoadmap,
                Title = "Roadmap Briefing",
                Description = "Closed-door look at next year's platform roadmap.",
                From = new DateTime(2026, 11, 15, 14, 0, 0, DateTimeKind.Utc),
                To = new DateTime(2026, 11, 15, 15, 0, 0, DateTimeKind.Utc),
                Level = SessionLevel.Advanced,
                Track = "Strategy",
                Room = "Boardroom",
                SortOrder = 0,
                SpeakerId = SeedData.Speakers.TomasBrandt
            },
            new()
            {
                Id = SeedData.Sessions.VipMa,
                Title = "M&A Panel",
                Description = "Industry leaders on consolidation in the developer tools space.",
                From = new DateTime(2026, 11, 15, 15, 30, 0, DateTimeKind.Utc),
                To = new DateTime(2026, 11, 15, 16, 30, 0, DateTimeKind.Utc),
                Level = SessionLevel.Advanced,
                Track = "Strategy",
                Room = "Boardroom",
                SortOrder = 1,
                SpeakerId = SeedData.Speakers.ElenaCosta
            },
            new()
            {
                Id = SeedData.Sessions.VipDinner,
                Title = "Networking Dinner",
                Description = "Structured introductions over dinner.",
                From = new DateTime(2026, 11, 15, 19, 0, 0, DateTimeKind.Utc),
                To = new DateTime(2026, 11, 15, 22, 0, 0, DateTimeKind.Utc),
                Level = SessionLevel.Beginner,
                Track = "Networking",
                Room = "Private Dining Room",
                SortOrder = 2,
                SpeakerId = null
            }
        }
    };

    /// <summary>
    /// Past event with closed registration — exercises the date-window gate.
    /// </summary>
    private static Event BuildPastSummit(DateTime createdAt) => new()
    {
        Id = SeedData.Events.PastSummit,
        Title = "Tech Summit 2025",
        Subtitle = "Last year's flagship event",
        Description = "Recordings and slides available on the archive page.",
        IsVip = false,
        Date = new DateOnly(2025, 9, 15),
        Hero = new Hero
        {
            Image = "https://example.com/events/past/hero.jpg",
            CtaText = "View Archive"
        },
        Location = new Location
        {
            City = "London",
            Venue = "ExCeL London",
            Address = "Royal Victoria Dock, London E16 1XL"
        },
        Registration = new RegistrationInfo
        {
            OpenDate = new DateOnly(2025, 6, 1),
            CloseDate = new DateOnly(2025, 9, 1),
            Fee = 79m,
            EarlyBirdDiscount = 15m
        },
        OrganizerId = SeedData.Users.OtherOrganizer,
        LayoutId = null,
        CreatedAt = createdAt,
        UpdatedAt = createdAt,
        Sessions = new List<Session>
        {
            new()
            {
                Id = SeedData.Sessions.PastRetro,
                Title = "Retrospective Keynote",
                Description = "A look back at 2025's biggest shifts in software.",
                From = new DateTime(2025, 9, 15, 9, 0, 0, DateTimeKind.Utc),
                To = new DateTime(2025, 9, 15, 10, 0, 0, DateTimeKind.Utc),
                Level = SessionLevel.Beginner,
                Track = "Keynote",
                Room = "Main Hall",
                SortOrder = 0,
                SpeakerId = SeedData.Speakers.JohnSmith
            },
            new()
            {
                Id = SeedData.Sessions.PastNetworking,
                Title = "Closing Reception",
                Description = "Drinks and farewells.",
                From = new DateTime(2025, 9, 15, 17, 0, 0, DateTimeKind.Utc),
                To = new DateTime(2025, 9, 15, 19, 0, 0, DateTimeKind.Utc),
                Level = SessionLevel.Beginner,
                Track = "Networking",
                Room = "Foyer",
                SortOrder = 1,
                SpeakerId = null
            }
        }
    };
}
