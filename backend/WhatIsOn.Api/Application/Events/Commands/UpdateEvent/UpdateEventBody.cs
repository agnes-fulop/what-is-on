namespace WhatIsOn.Application.Events.Commands.UpdateEvent;

/// <summary>
/// Wire format for PUT /api/events/{id}. The id comes from the URL; this is
/// the rest of the body the client sends. The controller combines them into
/// an <see cref="UpdateEventCommand"/>.
/// </summary>
public record UpdateEventBody(
    string Title,
    string Subtitle,
    string Description,
    bool IsVip,
    DateOnly Date,
    HeroInput Hero,
    LocationInput Location,
    RegistrationInfoInput Registration,
    Guid? LayoutId);
