using WhatIsOn.Domain.Entities;

namespace WhatIsOn.Domain.Interfaces;

public interface IEventRepository
{
    /// <summary>
    /// Returns events with their organizer eagerly loaded for the listing endpoint.
    /// Does not include sessions, registrations, or layout.
    /// </summary>
    Task<IReadOnlyList<Event>> GetSummariesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the event with organizer, sessions (and their speakers), and registrations
    /// eagerly loaded. The layout components are loaded separately via <see cref="ILayoutRepository"/>.
    /// </summary>
    Task<Event?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lean lookup that returns just the event row — used by paths (like
    /// registration) that don't need sessions, speakers, or organizer.
    /// </summary>
    Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Same as <see cref="GetByIdAsync"/> but tracked by the change tracker —
    /// used by the update path so EF can detect mutations on save.
    /// </summary>
    Task<Event?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Event @event, CancellationToken cancellationToken = default);
    void Update(Event @event);
}
