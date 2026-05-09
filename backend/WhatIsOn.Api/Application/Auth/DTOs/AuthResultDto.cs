namespace WhatIsOn.Application.Auth.DTOs;

public record AuthResultDto(
    string Token,
    DateTime ExpiresAtUtc,
    AuthenticatedUserDto User);
