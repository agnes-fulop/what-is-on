using WhatIsOn.Domain.Entities;
using WhatIsOn.Domain.Enums;

namespace WhatIsOn.Infrastructure.Persistence.Seed;

internal static class UsersSeed
{
    public static IReadOnlyList<User> Build(string passwordHash, DateTime createdAt)
    {
        return new[]
        {
            new User
            {
                Id = SeedData.Users.Organizer,
                Email = "organizer@example.com",
                DisplayName = "Demo Organizer",
                PasswordHash = passwordHash,
                Role = UserRole.Organizer,
                CreatedAt = createdAt
            },
            new User
            {
                Id = SeedData.Users.Regular,
                Email = "regular@example.com",
                DisplayName = "Regular User",
                PasswordHash = passwordHash,
                Role = UserRole.Regular,
                CreatedAt = createdAt
            },
            new User
            {
                Id = SeedData.Users.Vip,
                Email = "vip@example.com",
                DisplayName = "VIP User",
                PasswordHash = passwordHash,
                Role = UserRole.Vip,
                CreatedAt = createdAt
            },
            new User
            {
                Id = SeedData.Users.OtherOrganizer,
                Email = "organizer2@example.com",
                DisplayName = "Other Organizer",
                PasswordHash = passwordHash,
                Role = UserRole.Organizer,
                CreatedAt = createdAt
            }
        };
    }
}
