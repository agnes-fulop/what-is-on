using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WhatIsOn.Application.Auth.Interfaces;
using WhatIsOn.Domain.Interfaces;
using WhatIsOn.Infrastructure.Authentication;
using WhatIsOn.Infrastructure.Persistence;
using WhatIsOn.Infrastructure.Repositories;

namespace WhatIsOn.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddPersistence(services, configuration);
        AddAuthentication(services, configuration);
        AddRepositories(services);

        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

        services.AddDbContext<WhatIsOnDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<WhatIsOnDbContext>());
    }

    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<ILayoutRepository, LayoutRepository>();
        services.AddScoped<ISpeakerRepository, SpeakerRepository>();
    }
}
