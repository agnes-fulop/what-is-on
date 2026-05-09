using WhatIsOn.Domain.Entities;

namespace WhatIsOn.Domain.Interfaces;

public interface ISpeakerRepository
{
    /// <summary>
    /// Batch-loads speakers by id. Used by the layout enrichment to fill in
    /// SpeakerCard data without N+1 queries.
    /// </summary>
    Task<IReadOnlyList<Speaker>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}
