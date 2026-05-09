using WhatIsOn.Application.Auth.Interfaces;
using WhatIsOn.Application.Events.DTOs;
using WhatIsOn.Application.Layouts;
using WhatIsOn.Application.Layouts.DTOs;
using WhatIsOn.Domain.Entities;
using WhatIsOn.Domain.Exceptions;
using WhatIsOn.Domain.Interfaces;

namespace WhatIsOn.Application.Events.Queries.GetEventById;

public class GetEventByIdHandler
{
    private readonly IEventRepository _events;
    private readonly ILayoutRepository _layouts;
    private readonly ISpeakerRepository _speakers;
    private readonly LayoutTreeBuilder _treeBuilder;
    private readonly ICurrentUser _currentUser;

    public GetEventByIdHandler(
        IEventRepository events,
        ILayoutRepository layouts,
        ISpeakerRepository speakers,
        LayoutTreeBuilder treeBuilder,
        ICurrentUser currentUser)
    {
        _events = events;
        _layouts = layouts;
        _speakers = speakers;
        _treeBuilder = treeBuilder;
        _currentUser = currentUser;
    }

    public async Task<EventDetailDto> Handle(GetEventByIdQuery query, CancellationToken cancellationToken)
    {
        var entity = await _events.GetByIdWithDetailsAsync(query.EventId, cancellationToken)
            ?? throw new NotFoundException("Event", query.EventId);

        EventAccessPolicy.EnsureCanAccess(entity, _currentUser);

        var layoutDto = entity.LayoutId.HasValue
            ? await BuildLayoutAsync(entity.LayoutId.Value, entity.Sessions, cancellationToken)
            : null;

        return MapToDetail(entity, layoutDto);
    }

    private async Task<LayoutDto?> BuildLayoutAsync(
        Guid layoutId,
        ICollection<Session> eventSessions,
        CancellationToken cancellationToken)
    {
        var layout = await _layouts.GetByIdWithComponentsAsync(layoutId, cancellationToken);
        if (layout is null) return null;

        var speakerIds = LayoutTreeBuilder.ExtractSpeakerIds(layout.Components);
        var speakers = await _speakers.GetByIdsAsync(speakerIds, cancellationToken);

        var speakersById = speakers.ToDictionary(s => s.Id);
        var sessionsById = eventSessions.ToDictionary(s => s.Id);

        return _treeBuilder.Build(layout, speakersById, sessionsById);
    }

    private static EventDetailDto MapToDetail(Event entity, LayoutDto? layout) => new(
        entity.Id,
        entity.Title,
        entity.Subtitle,
        entity.Description,
        entity.IsVip,
        entity.Date,
        new HeroDto(entity.Hero.Image, entity.Hero.CtaText),
        new LocationDto(entity.Location.City, entity.Location.Venue, entity.Location.Address),
        new OrganizerDto(entity.Organizer.Id, entity.Organizer.DisplayName),
        entity.Sessions.Select(MapSession).ToList(),
        new RegistrationInfoDto(
            entity.Registration.OpenDate,
            entity.Registration.CloseDate,
            entity.Registration.Fee,
            entity.Registration.EarlyBirdDiscount),
        layout);

    private static SessionDto MapSession(Session session)
    {
        var speaker = session.Speaker is null ? null : new SpeakerDto(
            session.Speaker.Id, session.Speaker.Name, session.Speaker.Title,
            session.Speaker.Bio, session.Speaker.Image);

        return new SessionDto(
            session.Id, session.Title, session.Description, session.From, session.To,
            session.Level, session.Track, session.Room, speaker);
    }
}
