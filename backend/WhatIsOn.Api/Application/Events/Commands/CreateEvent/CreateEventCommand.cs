namespace WhatIsOn.Application.Events.Commands.CreateEvent;

public record CreateEventCommand(
    string Title,
    string Subtitle,
    string Description,
    bool IsVip,
    DateOnly Date,
    HeroInput Hero,
    LocationInput Location,
    RegistrationInfoInput Registration,
    Guid? LayoutId,
    IReadOnlyList<SessionInput>? Sessions);
