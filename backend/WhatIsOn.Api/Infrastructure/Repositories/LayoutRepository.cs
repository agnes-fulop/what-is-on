using Microsoft.EntityFrameworkCore;
using WhatIsOn.Domain.Entities;
using WhatIsOn.Domain.Interfaces;
using WhatIsOn.Infrastructure.Persistence;

namespace WhatIsOn.Infrastructure.Repositories;

public class LayoutRepository : ILayoutRepository
{
    private readonly WhatIsOnDbContext _context;

    public LayoutRepository(WhatIsOnDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Loads the layout and all of its components in a single round-trip.
    /// The flat list is reshaped into a tree by <see cref="Application.Layouts.LayoutTreeBuilder"/>.
    /// </summary>
    public Task<Layout?> GetByIdWithComponentsAsync(Guid layoutId, CancellationToken cancellationToken = default)
    {
        return _context.Layouts
            .AsNoTracking()
            .Include(l => l.Components)
            .FirstOrDefaultAsync(l => l.Id == layoutId, cancellationToken);
    }
}
