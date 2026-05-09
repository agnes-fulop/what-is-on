using WhatIsOn.Application.Events.DTOs;
using WhatIsOn.Domain.Entities;
using WhatIsOn.Domain.Interfaces;

namespace WhatIsOn.Application.Events.Queries.GetEventList;

public class GetEventListHandler
{
    private readonly IEventRepository _events;

    public GetEventListHandler(IEventRepository events)
    {
        _events = events;
    }

    public async Task<IReadOnlyList<EventSummaryDto>> Handle(CancellationToken cancellationToken)
    {
        var events = await _events.GetSummariesAsync(cancellationToken);
        return events.Select(MapToSummary).ToList();
    }

    private static EventSummaryDto MapToSummary(Event entity) => new(
        entity.Id,
        entity.Title,
        entity.Subtitle,
        entity.Date,
        entity.IsVip,
        new HeroDto(entity.Hero.Image, entity.Hero.CtaText),
        new LocationDto(entity.Location.City, entity.Location.Venue, entity.Location.Address),
        new OrganizerDto(entity.Organizer.Id, entity.Organizer.DisplayName),
        entity.Registration.Fee,
        entity.Registration.EarlyBirdDiscount);
}
