using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Shouldly;
using WhatIsOn.Application.Auth.Interfaces;
using WhatIsOn.Application.Registrations.Commands.RegisterForEvent;
using WhatIsOn.Domain.Entities;
using WhatIsOn.Domain.Enums;
using WhatIsOn.Domain.Exceptions;
using WhatIsOn.Domain.Interfaces;
using WhatIsOn.Domain.ValueObjects;

namespace WhatIsOn.Tests.Application.Registrations.Commands;

public class RegisterForEventHandlerTests
{
    private static readonly DateTimeOffset NowDuringWindow = new(2026, 5, 15, 0, 0, 0, TimeSpan.Zero);
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid EventId = Guid.NewGuid();

    private readonly IEventRepository _events = Substitute.For<IEventRepository>();
    private readonly IRegistrationRepository _registrations = Substitute.For<IRegistrationRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly FakeTimeProvider _clock = new(NowDuringWindow);

    [Fact]
    public async Task Handle_NotAuthenticated_ThrowsUnauthorized()
    {
        _currentUser.IsAuthenticated.Returns(false);
        _currentUser.UserId.Returns((Guid?)null);

        var ex = await Should.ThrowAsync<UnauthorizedException>(
            () => CreateHandler().Handle(new RegisterForEventCommand(EventId), default));
        ex.Message.ShouldContain("Authentication");
    }

    [Fact]
    public async Task Handle_EventNotFound_ThrowsNotFound()
    {
        AuthenticateAsRegular();
        _events.GetByIdAsync(EventId, Arg.Any<CancellationToken>()).Returns((Event?)null);

        await Should.ThrowAsync<NotFoundException>(
            () => CreateHandler().Handle(new RegisterForEventCommand(EventId), default));
    }

    [Fact]
    public async Task Handle_VipEvent_RegularUser_ThrowsForbidden()
    {
        AuthenticateAsRegular();
        _events.GetByIdAsync(EventId, Arg.Any<CancellationToken>()).Returns(VipEvent());

        await Should.ThrowAsync<ForbiddenException>(
            () => CreateHandler().Handle(new RegisterForEventCommand(EventId), default));
    }

    [Fact]
    public async Task Handle_RegistrationNotYetOpen_ThrowsValidation()
    {
        AuthenticateAsRegular();
        var futureEvent = PublicEvent();
        futureEvent.Registration = new RegistrationInfo
        {
            OpenDate = new DateOnly(2027, 1, 1),
            CloseDate = new DateOnly(2027, 12, 31),
            Fee = 0m,
            EarlyBirdDiscount = 0m,
        };
        _events.GetByIdAsync(EventId, Arg.Any<CancellationToken>()).Returns(futureEvent);

        var ex = await Should.ThrowAsync<ValidationException>(
            () => CreateHandler().Handle(new RegisterForEventCommand(EventId), default));
        ex.Message.ShouldContain("opens");
    }

    [Fact]
    public async Task Handle_RegistrationClosed_ThrowsValidation()
    {
        AuthenticateAsRegular();
        var pastEvent = PublicEvent();
        pastEvent.Registration = new RegistrationInfo
        {
            OpenDate = new DateOnly(2025, 1, 1),
            CloseDate = new DateOnly(2025, 6, 1),
            Fee = 0m,
            EarlyBirdDiscount = 0m,
        };
        _events.GetByIdAsync(EventId, Arg.Any<CancellationToken>()).Returns(pastEvent);

        var ex = await Should.ThrowAsync<ValidationException>(
            () => CreateHandler().Handle(new RegisterForEventCommand(EventId), default));
        ex.Message.ShouldContain("closed");
    }

    [Fact]
    public async Task Handle_DuplicateRegistration_ThrowsConflict()
    {
        AuthenticateAsRegular();
        _events.GetByIdAsync(EventId, Arg.Any<CancellationToken>()).Returns(PublicEvent());
        _registrations.ExistsAsync(EventId, UserId, Arg.Any<CancellationToken>()).Returns(true);

        await Should.ThrowAsync<ConflictException>(
            () => CreateHandler().Handle(new RegisterForEventCommand(EventId), default));
    }

    [Fact]
    public async Task Handle_HappyPath_PersistsAndReturnsDto()
    {
        AuthenticateAsRegular();
        _events.GetByIdAsync(EventId, Arg.Any<CancellationToken>()).Returns(PublicEvent());
        _registrations.ExistsAsync(EventId, UserId, Arg.Any<CancellationToken>()).Returns(false);

        var result = await CreateHandler().Handle(new RegisterForEventCommand(EventId), default);

        result.EventId.ShouldBe(EventId);
        result.UserId.ShouldBe(UserId);
        result.Id.ShouldNotBe(Guid.Empty);

        await _registrations.Received(1).AddAsync(
            Arg.Is<Registration>(r => r.EventId == EventId && r.UserId == UserId),
            Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_HappyPath_StampsRegisteredAtFromTimeProvider()
    {
        AuthenticateAsRegular();
        _events.GetByIdAsync(EventId, Arg.Any<CancellationToken>()).Returns(PublicEvent());
        _registrations.ExistsAsync(EventId, UserId, Arg.Any<CancellationToken>()).Returns(false);

        var result = await CreateHandler().Handle(new RegisterForEventCommand(EventId), default);

        // FakeTimeProvider's GetUtcNow().UtcDateTime should be reflected in the row.
        result.RegisteredAt.ShouldBe(NowDuringWindow.UtcDateTime);
    }

    private RegisterForEventHandler CreateHandler() =>
        new(_events, _registrations, _unitOfWork, _currentUser, _clock);

    private void AuthenticateAsRegular()
    {
        _currentUser.IsAuthenticated.Returns(true);
        _currentUser.UserId.Returns(UserId);
        _currentUser.Role.Returns(UserRole.Regular);
    }

    private static Event PublicEvent() => new()
    {
        Id = EventId,
        IsVip = false,
        Registration = new RegistrationInfo
        {
            OpenDate = new DateOnly(2026, 1, 1),
            CloseDate = new DateOnly(2026, 12, 31),
            Fee = 50m,
            EarlyBirdDiscount = 0m,
        },
    };

    private static Event VipEvent() => new()
    {
        Id = EventId,
        IsVip = true,
        Registration = new RegistrationInfo
        {
            OpenDate = new DateOnly(2026, 1, 1),
            CloseDate = new DateOnly(2026, 12, 31),
            Fee = 200m,
            EarlyBirdDiscount = 0m,
        },
    };
}
