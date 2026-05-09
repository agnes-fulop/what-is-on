using Microsoft.Extensions.DependencyInjection;
using WhatIsOn.Application.Auth.Commands.Login;
using WhatIsOn.Application.Auth.Commands.Register;
using WhatIsOn.Application.Events.Queries.GetEventById;
using WhatIsOn.Application.Events.Queries.GetEventList;
using WhatIsOn.Application.Layouts;

namespace WhatIsOn.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<RegisterHandler>();
        services.AddScoped<LoginHandler>();

        services.AddScoped<GetEventListHandler>();
        services.AddScoped<GetEventByIdHandler>();

        services.AddSingleton<LayoutTreeBuilder>();

        return services;
    }
}
