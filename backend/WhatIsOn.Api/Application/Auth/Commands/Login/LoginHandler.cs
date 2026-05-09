using WhatIsOn.Application.Auth.DTOs;
using WhatIsOn.Application.Auth.Interfaces;
using WhatIsOn.Domain.Exceptions;
using WhatIsOn.Domain.Interfaces;

namespace WhatIsOn.Application.Auth.Commands.Login;

public class LoginHandler
{
    private const string InvalidCredentialsMessage = "Invalid email or password.";

    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginHandler(
        IUserRepository users,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResultDto> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Email) || string.IsNullOrWhiteSpace(command.Password))
        {
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        var user = await _users.GetByEmailAsync(normalizedEmail, cancellationToken);

        // Generic message in both branches so we don't leak which accounts exist.
        if (user is null || !_passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        var token = _tokenService.IssueToken(user);

        return new AuthResultDto(
            token.Token,
            token.ExpiresAtUtc,
            new AuthenticatedUserDto(user.Id, user.Email, user.DisplayName, user.Role));
    }
}
