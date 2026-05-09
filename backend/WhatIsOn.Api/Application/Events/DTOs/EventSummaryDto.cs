namespace WhatIsOn.Application.Events.DTOs;

/// <summary>
/// Lightweight projection used by the event listing endpoint. Excludes sessions,
/// registrations, and the layout — those are only loaded for the detail view.
/// </summary>
public record EventSummaryDto(
    Guid Id,
    string Title,
    string Subtitle,
    DateOnly Date,
    bool IsVip,
    HeroDto Hero,
    LocationDto Location,
    OrganizerDto Organizer,
    decimal RegistrationFee,
    decimal EarlyBirdDiscount);
