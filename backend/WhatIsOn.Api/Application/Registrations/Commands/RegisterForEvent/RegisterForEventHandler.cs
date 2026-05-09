using WhatIsOn.Application.Auth.Interfaces;
using WhatIsOn.Application.Events;
using WhatIsOn.Application.Registrations.DTOs;
using WhatIsOn.Domain.Entities;
using WhatIsOn.Domain.Exceptions;
using WhatIsOn.Domain.Interfaces;

namespace WhatIsOn.Application.Registrations.Commands.RegisterForEvent;

public class RegisterForEventHandler
{
    private readonly IEventRepository _events;
    private readonly IRegistrationRepository _registrations;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _clock;

    public RegisterForEventHandler(
        IEventRepository events,
        IRegistrationRepository registrations,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        TimeProvider clock)
    {
        _events = events;
        _registrations = registrations;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _clock = clock;
    }

    public async Task<RegistrationDto> Handle(RegisterForEventCommand command, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is not { } userId)
        {
            throw new UnauthorizedException("Authentication required to register for an event.");
        }

        var @event = await _events.GetByIdAsync(command.EventId, cancellationToken)
            ?? throw new NotFoundException("Event", command.EventId);

        EventAccessPolicy.EnsureCanAccess(@event, _currentUser);
        EnsureRegistrationWindowOpen(@event);

        if (await _registrations.ExistsAsync(@event.Id, userId, cancellationToken))
        {
            throw new ConflictException("You are already registered for this event.");
        }

        var registration = new Registration
        {
            Id = Guid.NewGuid(),
            EventId = @event.Id,
            UserId = userId,
            RegisteredAt = _clock.GetUtcNow().UtcDateTime
        };

        await _registrations.AddAsync(registration, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegistrationDto(
            registration.Id,
            registration.EventId,
            registration.UserId,
            registration.RegisteredAt);
    }

    private void EnsureRegistrationWindowOpen(Event @event)
    {
        var today = DateOnly.FromDateTime(_clock.GetUtcNow().UtcDateTime);

        if (today < @event.Registration.OpenDate)
        {
            throw new ValidationException(
                $"Registration opens on {@event.Registration.OpenDate:yyyy-MM-dd}.");
        }

        if (today > @event.Registration.CloseDate)
        {
            throw new ValidationException(
                $"Registration closed on {@event.Registration.CloseDate:yyyy-MM-dd}.");
        }
    }
}
