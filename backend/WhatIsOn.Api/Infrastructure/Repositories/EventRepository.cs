using Microsoft.EntityFrameworkCore;
using WhatIsOn.Domain.Entities;
using WhatIsOn.Domain.Interfaces;
using WhatIsOn.Infrastructure.Persistence;

namespace WhatIsOn.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly WhatIsOnDbContext _context;

    public EventRepository(WhatIsOnDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Event>> GetSummariesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .AsNoTracking()
            .Include(e => e.Organizer)
            .OrderBy(e => e.Date)
            .ToListAsync(cancellationToken);
    }

    public Task<Event?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Events
            .AsNoTracking()
            .Include(e => e.Organizer)
            .Include(e => e.Sessions.OrderBy(s => s.SortOrder))
                .ThenInclude(s => s.Speaker)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task AddAsync(Event @event, CancellationToken cancellationToken = default)
    {
        await _context.Events.AddAsync(@event, cancellationToken);
    }

    public void Update(Event @event)
    {
        _context.Events.Update(@event);
    }
}
