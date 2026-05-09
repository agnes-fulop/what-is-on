using WhatIsOn.Application.Auth.Interfaces;
using WhatIsOn.Domain.Entities;
using WhatIsOn.Domain.Exceptions;
using WhatIsOn.Domain.Interfaces;
using WhatIsOn.Domain.ValueObjects;

namespace WhatIsOn.Application.Events.Commands.CreateEvent;

public class CreateEventHandler
{
    private readonly IEventRepository _events;
    private readonly ILayoutRepository _layouts;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _clock;

    public CreateEventHandler(
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

    public async Task<Guid> Handle(CreateEventCommand command, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } organizerId)
        {
            throw new UnauthorizedException("Authentication required.");
        }

        EventInputValidator.ValidateMetadata(command.Title, command.Registration);
        EventInputValidator.ValidateSessions(command.Sessions);

        if (command.LayoutId.HasValue)
        {
            var layout = await _layouts.GetByIdWithComponentsAsync(command.LayoutId.Value, cancellationToken);
            if (layout is null)
            {
                throw new NotFoundException("Layout", command.LayoutId.Value);
            }
        }

        var now = _clock.GetUtcNow().UtcDateTime;
        var entity = new Event
        {
            Id = Guid.NewGuid(),
            Title = command.Title.Trim(),
            Subtitle = command.Subtitle ?? string.Empty,
            Description = command.Description ?? string.Empty,
            IsVip = command.IsVip,
            Date = command.Date,
            Hero = new Hero { Image = command.Hero.Image, CtaText = command.Hero.CtaText },
            Location = new Location
            {
                City = command.Location.City,
                Venue = command.Location.Venue,
                Address = command.Location.Address
            },
            Registration = new RegistrationInfo
            {
                OpenDate = command.Registration.OpenDate,
                CloseDate = command.Registration.CloseDate,
                Fee = command.Registration.Fee,
                EarlyBirdDiscount = command.Registration.EarlyBirdDiscount
            },
            OrganizerId = organizerId,
            LayoutId = command.LayoutId,
            CreatedAt = now,
            UpdatedAt = now,
            Sessions = MapSessions(command.Sessions)
        };

        await _events.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    private static List<Session> MapSessions(IReadOnlyList<SessionInput>? sessions)
    {
        if (sessions is null || sessions.Count == 0) return new List<Session>();

        return sessions
            .Select((input, index) => new Session
            {
                Id = Guid.NewGuid(),
                Title = input.Title.Trim(),
                Description = input.Description ?? string.Empty,
                From = input.From,
                To = input.To,
                Level = input.Level,
                Track = input.Track ?? string.Empty,
                Room = input.Room ?? string.Empty,
                SpeakerId = input.SpeakerId,
                SortOrder = index
            })
            .ToList();
    }
}
