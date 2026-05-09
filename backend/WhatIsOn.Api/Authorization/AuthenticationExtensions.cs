using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WhatIsOn.Application.Auth.Interfaces;
using WhatIsOn.Api.Services;
using WhatIsOn.Domain.Enums;
using WhatIsOn.Infrastructure.Authentication;

namespace WhatIsOn.Api.Authorization;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddApiAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwt = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("Jwt configuration section is missing.");

        if (string.IsNullOrWhiteSpace(jwt.Key))
        {
            throw new InvalidOperationException(
                "Jwt:Key is not configured. Set it via 'dotnet user-secrets set Jwt:Key <value>' " +
                "for local development, or via the Jwt__Key environment variable.");
        }

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicies.VipOrOrganizer, policy =>
                policy.RequireAuthenticatedUser()
                      .RequireRole(UserRole.Vip.ToString(), UserRole.Organizer.ToString()));

            options.AddPolicy(AuthorizationPolicies.OrganizerOnly, policy =>
                policy.RequireAuthenticatedUser()
                      .RequireRole(UserRole.Organizer.ToString()));
        });

        return services;
    }
}
