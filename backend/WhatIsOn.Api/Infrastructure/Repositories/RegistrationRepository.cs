using Microsoft.EntityFrameworkCore;
using WhatIsOn.Domain.Entities;
using WhatIsOn.Domain.Interfaces;
using WhatIsOn.Infrastructure.Persistence;

namespace WhatIsOn.Infrastructure.Repositories;

public class RegistrationRepository : IRegistrationRepository
{
    private readonly WhatIsOnDbContext _context;

    public RegistrationRepository(WhatIsOnDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default) =>
        _context.Registrations.AnyAsync(
            r => r.EventId == eventId && r.UserId == userId,
            cancellationToken);

    public async Task AddAsync(Registration registration, CancellationToken cancellationToken = default)
    {
        await _context.Registrations.AddAsync(registration, cancellationToken);
    }
}
