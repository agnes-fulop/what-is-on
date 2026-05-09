using WhatIsOn.Domain.Exceptions;

namespace WhatIsOn.Application.Events.Commands;

/// <summary>
/// Shared validation for the create + update event paths. Validation rules
/// live in one place so the two write paths cannot drift.
/// </summary>
internal static class EventInputValidator
{
    public static void ValidateMetadata(string title, RegistrationInfoInput registration)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ValidationException("Event title is required.");
        }

        if (registration.OpenDate > registration.CloseDate)
        {
            throw new ValidationException(
                "Registration open date must be on or before the close date.");
        }

        if (registration.Fee < 0)
        {
            throw new ValidationException("Registration fee cannot be negative.");
        }

        if (registration.EarlyBirdDiscount < 0 || registration.EarlyBirdDiscount > registration.Fee)
        {
            throw new ValidationException(
                "Early bird discount must be between 0 and the registration fee.");
        }
    }

    public static void ValidateSessions(IEnumerable<SessionInput>? sessions)
    {
        if (sessions is null) return;

        foreach (var session in sessions)
        {
            if (string.IsNullOrWhiteSpace(session.Title))
            {
                throw new ValidationException("Each session must have a title.");
            }

            if (session.From >= session.To)
            {
                throw new ValidationException(
                    $"Session '{session.Title}' must end after it starts.");
            }
        }
    }
}
