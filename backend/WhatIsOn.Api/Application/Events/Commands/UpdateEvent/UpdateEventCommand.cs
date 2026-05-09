namespace WhatIsOn.Application.Events.Commands.UpdateEvent;

/// <summary>
/// Updates the metadata of an existing event. Sessions are not touched —
/// session management is intentionally out of scope for this command.
/// </summary>
public record UpdateEventCommand(
    Guid EventId,
    string Title,
    string Subtitle,
    string Description,
    bool IsVip,
    DateOnly Date,
    HeroInput Hero,
    LocationInput Location,
    RegistrationInfoInput Registration,
    Guid? LayoutId);
