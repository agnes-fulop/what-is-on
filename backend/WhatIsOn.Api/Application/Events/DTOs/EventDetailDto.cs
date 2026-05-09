using WhatIsOn.Application.Layouts.DTOs;

namespace WhatIsOn.Application.Events.DTOs;

public record EventDetailDto(
    Guid Id,
    string Title,
    string Subtitle,
    string Description,
    bool IsVip,
    DateOnly Date,
    HeroDto Hero,
    LocationDto Location,
    OrganizerDto Organizer,
    IReadOnlyList<SessionDto> Sessions,
    RegistrationInfoDto Registration,
    LayoutDto? Layout);
