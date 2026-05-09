using Shouldly;
using WhatIsOn.Application.Events.Commands;
using WhatIsOn.Domain.Enums;
using WhatIsOn.Domain.Exceptions;

namespace WhatIsOn.Tests.Application.Events.Commands;

public class EventInputValidatorTests
{
    [Fact]
    public void ValidateMetadata_ValidInput_DoesNotThrow()
    {
        Should.NotThrow(() => EventInputValidator.ValidateMetadata("Conf", ValidRegistration()));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ValidateMetadata_EmptyTitle_Throws(string? title)
    {
        Should.Throw<ValidationException>(
            () => EventInputValidator.ValidateMetadata(title!, ValidRegistration()));
    }

    [Fact]
    public void ValidateMetadata_OpenAfterClose_Throws()
    {
        var registration = new RegistrationInfoInput(
            OpenDate: new DateOnly(2026, 6, 1),
            CloseDate: new DateOnly(2026, 5, 1),
            Fee: 100m,
            EarlyBirdDiscount: 0m);

        Should.Throw<ValidationException>(
            () => EventInputValidator.ValidateMetadata("Conf", registration));
    }

    [Fact]
    public void ValidateMetadata_OpenEqualsClose_DoesNotThrow()
    {
        var date = new DateOnly(2026, 6, 1);
        var registration = new RegistrationInfoInput(date, date, 100m, 0m);

        Should.NotThrow(() => EventInputValidator.ValidateMetadata("Conf", registration));
    }

    [Fact]
    public void ValidateMetadata_NegativeFee_Throws()
    {
        var registration = ValidRegistration() with { Fee = -1m };

        Should.Throw<ValidationException>(
            () => EventInputValidator.ValidateMetadata("Conf", registration));
    }

    [Fact]
    public void ValidateMetadata_NegativeDiscount_Throws()
    {
        var registration = ValidRegistration() with { EarlyBirdDiscount = -1m };

        Should.Throw<ValidationException>(
            () => EventInputValidator.ValidateMetadata("Conf", registration));
    }

    [Fact]
    public void ValidateMetadata_DiscountGreaterThanFee_Throws()
    {
        var registration = ValidRegistration() with { Fee = 50m, EarlyBirdDiscount = 100m };

        Should.Throw<ValidationException>(
            () => EventInputValidator.ValidateMetadata("Conf", registration));
    }

    [Fact]
    public void ValidateSessions_NullList_DoesNotThrow()
    {
        Should.NotThrow(() => EventInputValidator.ValidateSessions(null));
    }

    [Fact]
    public void ValidateSessions_EmptyList_DoesNotThrow()
    {
        Should.NotThrow(() => EventInputValidator.ValidateSessions(Array.Empty<SessionInput>()));
    }

    [Fact]
    public void ValidateSessions_ValidList_DoesNotThrow()
    {
        var sessions = new[] { ValidSession("Session A"), ValidSession("Session B") };

        Should.NotThrow(() => EventInputValidator.ValidateSessions(sessions));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void ValidateSessions_EmptyTitle_Throws(string title)
    {
        var sessions = new[] { ValidSession(title) };

        Should.Throw<ValidationException>(() => EventInputValidator.ValidateSessions(sessions));
    }

    [Fact]
    public void ValidateSessions_FromAfterTo_Throws()
    {
        var session = ValidSession("Reversed") with
        {
            From = new DateTime(2026, 1, 1, 11, 0, 0, DateTimeKind.Utc),
            To = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc),
        };

        Should.Throw<ValidationException>(() => EventInputValidator.ValidateSessions(new[] { session }));
    }

    [Fact]
    public void ValidateSessions_FromEqualToTo_Throws()
    {
        var instant = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var session = ValidSession("Zero-length") with { From = instant, To = instant };

        Should.Throw<ValidationException>(() => EventInputValidator.ValidateSessions(new[] { session }));
    }

    private static RegistrationInfoInput ValidRegistration() => new(
        OpenDate: new DateOnly(2026, 1, 1),
        CloseDate: new DateOnly(2026, 12, 31),
        Fee: 100m,
        EarlyBirdDiscount: 20m);

    private static SessionInput ValidSession(string title) => new(
        Title: title,
        Description: "desc",
        From: new DateTime(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc),
        To: new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc),
        Level: SessionLevel.Beginner,
        Track: "Track",
        Room: "Room",
        SpeakerId: null);
}
