using WhatIsOn.Domain.Entities;

namespace WhatIsOn.Infrastructure.Persistence.Seed;

internal static class SpeakersSeed
{
    public static IReadOnlyList<Speaker> Build()
    {
        return new[]
        {
            new Speaker
            {
                Id = SeedData.Speakers.JaneDoe,
                Name = "Dr. Jane Doe",
                Title = "VP of Engineering, Example Corp",
                Bio = "Expert in serverless architectures and cloud-native development.",
                Image = "https://example.com/speakers/jane.jpg"
            },
            new Speaker
            {
                Id = SeedData.Speakers.JohnSmith,
                Name = "John Smith",
                Title = "CTO, CloudCo",
                Bio = "Builder of large-scale distributed systems. Loves tracing and chaos engineering.",
                Image = "https://example.com/speakers/john.jpg"
            },
            new Speaker
            {
                Id = SeedData.Speakers.AishaKhan,
                Name = "Aisha Khan",
                Title = "Principal Engineer, ML Platform",
                Bio = "Leads the ML inference platform serving millions of requests per minute.",
                Image = "https://example.com/speakers/aisha.jpg"
            },
            new Speaker
            {
                Id = SeedData.Speakers.MarcusLee,
                Name = "Marcus Lee",
                Title = "DevRel Lead, Frontend Foundation",
                Bio = "Helps developers fall in love with the web platform.",
                Image = "https://example.com/speakers/marcus.jpg"
            },
            new Speaker
            {
                Id = SeedData.Speakers.ElenaCosta,
                Name = "Elena Costa",
                Title = "Security Architect",
                Bio = "Threat modeling, supply chain security, and zero-trust networks.",
                Image = "https://example.com/speakers/elena.jpg"
            },
            new Speaker
            {
                Id = SeedData.Speakers.TomasBrandt,
                Name = "Tomas Brandt",
                Title = "Product Director",
                Bio = "Two decades of shipping enterprise software. Strategy and roadmap.",
                Image = "https://example.com/speakers/tomas.jpg"
            }
        };
    }
}
