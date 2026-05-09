namespace WhatIsOn.Application.Events.DTOs;

public record SpeakerDto(
    Guid Id,
    string Name,
    string Title,
    string Bio,
    string Image);
