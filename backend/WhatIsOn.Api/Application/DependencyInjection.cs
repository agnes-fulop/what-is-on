using Microsoft.Extensions.DependencyInjection;
using WhatIsOn.Application.Auth.Commands.Login;
using WhatIsOn.Application.Auth.Commands.Register;

namespace WhatIsOn.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<RegisterHandler>();
        services.AddScoped<LoginHandler>();

        return services;
    }
}
