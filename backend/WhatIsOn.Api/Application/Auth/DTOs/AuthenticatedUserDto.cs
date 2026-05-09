using WhatIsOn.Domain.Enums;

namespace WhatIsOn.Application.Auth.DTOs;

public record AuthenticatedUserDto(
    Guid Id,
    string Email,
    string DisplayName,
    UserRole Role);
