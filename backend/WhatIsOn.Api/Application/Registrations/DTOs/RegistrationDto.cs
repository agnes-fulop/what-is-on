namespace WhatIsOn.Application.Registrations.DTOs;

public record RegistrationDto(
    Guid Id,
    Guid EventId,
    Guid UserId,
    DateTime RegisteredAt);
