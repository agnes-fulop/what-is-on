namespace WhatIsOn.Application.Events.DTOs;

public record RegistrationInfoDto(
    DateOnly OpenDate,
    DateOnly CloseDate,
    decimal Fee,
    decimal EarlyBirdDiscount);
