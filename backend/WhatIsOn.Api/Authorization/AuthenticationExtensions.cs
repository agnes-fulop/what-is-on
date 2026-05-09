using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WhatIsOn.Application.Auth.Interfaces;
using WhatIsOn.Api.Services;
using WhatIsOn.Domain.Enums;
using WhatIsOn.Infrastructure.Authentication;

namespace WhatIsOn.Api.Authorization;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddApiAuthentication(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        // Bind JwtBearerOptions lazily through IOptions<JwtSettings> so test
        // hosts can override Jwt:Key/Issuer/Audience via configuration without
        // racing the eager bind that used to happen at registration time.
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtSettings>>((options, jwtOptions) =>
            {
                var jwt = jwtOptions.Value;
                EnsureKeyConfigured(jwt);

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

    private static void EnsureKeyConfigured(JwtSettings jwt)
    {
        if (string.IsNullOrWhiteSpace(jwt.Key))
        {
            throw new InvalidOperationException(
                "Jwt:Key is not configured. Set it via 'dotnet user-secrets set Jwt:Key <value>' " +
                "for local development, or via the Jwt__Key environment variable.");
        }
    }
}
