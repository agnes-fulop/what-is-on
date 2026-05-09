using Microsoft.EntityFrameworkCore;
using WhatIsOn.Domain.Entities;
using WhatIsOn.Domain.Interfaces;
using WhatIsOn.Infrastructure.Persistence;

namespace WhatIsOn.Infrastructure.Repositories;

public class SpeakerRepository : ISpeakerRepository
{
    private readonly WhatIsOnDbContext _context;

    public SpeakerRepository(WhatIsOnDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Speaker>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idSet = ids.ToHashSet();
        if (idSet.Count == 0)
        {
            return Array.Empty<Speaker>();
        }

        return await _context.Speakers
            .AsNoTracking()
            .Where(s => idSet.Contains(s.Id))
            .ToListAsync(cancellationToken);
    }
}
