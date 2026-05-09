using WhatIsOn.Domain.Enums;

namespace WhatIsOn.Application.Events.Commands;

public record HeroInput(string Image, string CtaText);

public record LocationInput(string City, string Venue, string Address);

public record RegistrationInfoInput(
    DateOnly OpenDate,
    DateOnly CloseDate,
    decimal Fee,
    decimal EarlyBirdDiscount);

public record SessionInput(
    string Title,
    string Description,
    DateTime From,
    DateTime To,
    SessionLevel Level,
    string Track,
    string Room,
    Guid? SpeakerId);
