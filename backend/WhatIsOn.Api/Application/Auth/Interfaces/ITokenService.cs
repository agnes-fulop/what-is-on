using WhatIsOn.Domain.Entities;

namespace WhatIsOn.Application.Auth.Interfaces;

public interface ITokenService
{
    /// <summary>
    /// Issues a signed JWT for the given user. Returns the token string and the
    /// absolute UTC instant at which it expires.
    /// </summary>
    IssuedToken IssueToken(User user);
}

public record IssuedToken(string Token, DateTime ExpiresAtUtc);
