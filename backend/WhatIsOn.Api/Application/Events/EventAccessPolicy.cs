using WhatIsOn.Application.Auth.Interfaces;
using WhatIsOn.Domain.Entities;
using WhatIsOn.Domain.Enums;
using WhatIsOn.Domain.Exceptions;

namespace WhatIsOn.Application.Events;

/// <summary>
/// Centralizes the rule for who can view and interact with a VIP-gated event.
/// Used by both the read and registration paths so they cannot drift.
/// </summary>
public static class EventAccessPolicy
{
    public static void EnsureCanAccess(Event @event, ICurrentUser currentUser)
    {
        if (!@event.IsVip) return;

        if (!currentUser.IsAuthenticated)
        {
            throw new UnauthorizedException("This event is restricted to VIP members. Please sign in.");
        }

        if (currentUser.Role is not (UserRole.Vip or UserRole.Organizer))
        {
            throw new ForbiddenException("This event is restricted to VIP members.");
        }
    }
}
