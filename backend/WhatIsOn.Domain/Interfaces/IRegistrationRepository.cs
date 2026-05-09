using WhatIsOn.Domain.Entities;

namespace WhatIsOn.Domain.Interfaces;

public interface IRegistrationRepository
{
    Task<bool> ExistsAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Registration registration, CancellationToken cancellationToken = default);
}
