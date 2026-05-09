using WhatIsOn.Application.Auth.Interfaces;
using WhatIsOn.Domain.Entities;
using WhatIsOn.Domain.Exceptions;
using WhatIsOn.Domain.Interfaces;

namespace WhatIsOn.Application.Events.Commands.UpdateEvent;

public class UpdateEventHandler
{
    private readonly IEventRepository _events;
    private readonly ILayoutRepository _layouts;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _clock;

    public UpdateEventHandler(
        IEventRepository events,
        ILayoutRepository layouts,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        TimeProvider clock)
    {
        _events = events;
        _layouts = layouts;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _clock = clock;
    }

    public async Task Handle(UpdateEventCommand command, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
        {
            throw new UnauthorizedException("Authentication required.");
        }

        EventInputValidator.ValidateMetadata(command.Title, command.Registration);

        var entity = await _events.GetTrackedByIdAsync(command.EventId, cancellationToken)
            ?? throw new NotFoundException("Event", command.EventId);

        if (entity.OrganizerId != userId)
        {
            throw new ForbiddenException("Only the event organizer can edit this event.");
        }

        if (command.LayoutId.HasValue && command.LayoutId != entity.LayoutId)
        {
            var layout = await _layouts.GetByIdWithComponentsAsync(command.LayoutId.Value, cancellationToken);
            if (layout is null)
            {
                throw new NotFoundException("Layout", command.LayoutId.Value);
            }
        }

        ApplyUpdates(entity, command);
        entity.UpdatedAt = _clock.GetUtcNow().UtcDateTime;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static void ApplyUpdates(Event entity, UpdateEventCommand command)
    {
        entity.Title = command.Title.Trim();
        entity.Subtitle = command.Subtitle ?? string.Empty;
        entity.Description = command.Description ?? string.Empty;
        entity.IsVip = command.IsVip;
        entity.Date = command.Date;

        entity.Hero.Image = command.Hero.Image;
        entity.Hero.CtaText = command.Hero.CtaText;

        entity.Location.City = command.Location.City;
        entity.Location.Venue = command.Location.Venue;
        entity.Location.Address = command.Location.Address;

        entity.Registration.OpenDate = command.Registration.OpenDate;
        entity.Registration.CloseDate = command.Registration.CloseDate;
        entity.Registration.Fee = command.Registration.Fee;
        entity.Registration.EarlyBirdDiscount = command.Registration.EarlyBirdDiscount;

        entity.LayoutId = command.LayoutId;
    }
}
