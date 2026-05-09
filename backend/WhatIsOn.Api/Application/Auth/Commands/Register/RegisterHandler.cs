using WhatIsOn.Application.Auth.DTOs;
using WhatIsOn.Application.Auth.Interfaces;
using WhatIsOn.Domain.Entities;
using WhatIsOn.Domain.Enums;
using WhatIsOn.Domain.Exceptions;
using WhatIsOn.Domain.Interfaces;

namespace WhatIsOn.Application.Auth.Commands.Register;

public class RegisterHandler
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterHandler(
        IUserRepository users,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResultDto> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        ValidateInput(command);

        var normalizedEmail = command.Email.Trim().ToLowerInvariant();

        if (await _users.EmailExistsAsync(normalizedEmail, cancellationToken))
        {
            throw new ConflictException($"An account with email '{normalizedEmail}' already exists.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            DisplayName = command.DisplayName.Trim(),
            PasswordHash = _passwordHasher.Hash(command.Password),
            Role = UserRole.Regular,
            CreatedAt = DateTime.UtcNow
        };

        await _users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _tokenService.IssueToken(user);

        return new AuthResultDto(
            token.Token,
            token.ExpiresAtUtc,
            new AuthenticatedUserDto(user.Id, user.Email, user.DisplayName, user.Role));
    }

    private static void ValidateInput(RegisterCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
        {
            throw new ValidationException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(command.DisplayName))
        {
            throw new ValidationException("Display name is required.");
        }

        if (string.IsNullOrWhiteSpace(command.Password) || command.Password.Length < 8)
        {
            throw new ValidationException("Password must be at least 8 characters.");
        }
    }
}
