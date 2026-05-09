using NSubstitute;
using Shouldly;
using WhatIsOn.Application.Auth.Interfaces;
using WhatIsOn.Application.Events;
using WhatIsOn.Domain.Entities;
using WhatIsOn.Domain.Enums;
using WhatIsOn.Domain.Exceptions;

namespace WhatIsOn.Tests.Application.Events;

public class EventAccessPolicyTests
{
    [Fact]
    public void NonVipEvent_AllowsAnyone_EvenAnonymous()
    {
        var anonymous = AnonymousUser();
        var publicEvent = new Event { IsVip = false };

        Should.NotThrow(() => EventAccessPolicy.EnsureCanAccess(publicEvent, anonymous));
    }

    [Fact]
    public void VipEvent_Anonymous_ThrowsUnauthorized()
    {
        var anonymous = AnonymousUser();
        var vipEvent = new Event { IsVip = true };

        Should.Throw<UnauthorizedException>(
            () => EventAccessPolicy.EnsureCanAccess(vipEvent, anonymous));
    }

    [Fact]
    public void VipEvent_RegularUser_ThrowsForbidden()
    {
        var regular = AuthedUser(UserRole.Regular);
        var vipEvent = new Event { IsVip = true };

        Should.Throw<ForbiddenException>(
            () => EventAccessPolicy.EnsureCanAccess(vipEvent, regular));
    }

    [Fact]
    public void VipEvent_VipUser_DoesNotThrow()
    {
        var vip = AuthedUser(UserRole.Vip);
        var vipEvent = new Event { IsVip = true };

        Should.NotThrow(() => EventAccessPolicy.EnsureCanAccess(vipEvent, vip));
    }

    [Fact]
    public void VipEvent_OrganizerUser_DoesNotThrow()
    {
        var organizer = AuthedUser(UserRole.Organizer);
        var vipEvent = new Event { IsVip = true };

        Should.NotThrow(() => EventAccessPolicy.EnsureCanAccess(vipEvent, organizer));
    }

    private static ICurrentUser AnonymousUser()
    {
        var user = Substitute.For<ICurrentUser>();
        user.IsAuthenticated.Returns(false);
        user.UserId.Returns((Guid?)null);
        user.Role.Returns((UserRole?)null);
        return user;
    }

    private static ICurrentUser AuthedUser(UserRole role)
    {
        var user = Substitute.For<ICurrentUser>();
        user.IsAuthenticated.Returns(true);
        user.UserId.Returns(Guid.NewGuid());
        user.Role.Returns(role);
        return user;
    }
}
