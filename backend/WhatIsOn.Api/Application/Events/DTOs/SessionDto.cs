using WhatIsOn.Domain.Enums;

namespace WhatIsOn.Application.Events.DTOs;

public record SessionDto(
    Guid Id,
    string Title,
    string Description,
    DateTime From,
    DateTime To,
    SessionLevel Level,
    string Track,
    string Room,
    SpeakerDto? Speaker);
