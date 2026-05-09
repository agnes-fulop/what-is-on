using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatIsOn.Application.Registrations.Commands.RegisterForEvent;
using WhatIsOn.Application.Registrations.DTOs;

namespace WhatIsOn.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/events/{eventId:guid}/registrations")]
public class RegistrationsController : ControllerBase
{
    private readonly RegisterForEventHandler _registerHandler;

    public RegistrationsController(RegisterForEventHandler registerHandler)
    {
        _registerHandler = registerHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(RegistrationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RegistrationDto>> Register(
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var registration = await _registerHandler.Handle(
            new RegisterForEventCommand(eventId), cancellationToken);

        return CreatedAtAction(
            actionName: nameof(Register),
            routeValues: new { eventId },
            value: registration);
    }
}
