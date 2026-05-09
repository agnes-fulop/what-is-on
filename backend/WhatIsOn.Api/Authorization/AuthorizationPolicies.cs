namespace WhatIsOn.Api.Authorization;

public static class AuthorizationPolicies
{
    public const string VipOrOrganizer = nameof(VipOrOrganizer);
    public const string OrganizerOnly = nameof(OrganizerOnly);
}
