using WhatIsOn.Domain.Entities;

namespace WhatIsOn.Domain.Interfaces;

public interface ILayoutRepository
{
    /// <summary>
    /// Returns the layout with all of its components as a flat list ordered by SortOrder.
    /// The component tree is reconstructed in-memory by the LayoutTreeBuilder.
    /// </summary>
    Task<Layout?> GetByIdWithComponentsAsync(Guid layoutId, CancellationToken cancellationToken = default);
}
