using WhatIsOn.Domain.Enums;

namespace WhatIsOn.Application.Auth.Interfaces;

/// <summary>
/// Provides access to the currently authenticated user from anywhere in the
/// Application layer without depending on HTTP plumbing. Implemented by the API
/// layer using IHttpContextAccessor.
/// </summary>
public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    Guid? UserId { get; }
    UserRole? Role { get; }
}
