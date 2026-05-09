using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhatIsOn.Api.Authorization;
using WhatIsOn.Application.Events.Commands.CreateEvent;
using WhatIsOn.Application.Events.Commands.UpdateEvent;
using WhatIsOn.Application.Events.DTOs;
using WhatIsOn.Application.Events.Queries.GetEventById;
using WhatIsOn.Application.Events.Queries.GetEventList;

namespace WhatIsOn.Api.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly GetEventListHandler _listHandler;
    private readonly GetEventByIdHandler _byIdHandler;
    private readonly CreateEventHandler _createHandler;
    private readonly UpdateEventHandler _updateHandler;

    public EventsController(
        GetEventListHandler listHandler,
        GetEventByIdHandler byIdHandler,
        CreateEventHandler createHandler,
        UpdateEventHandler updateHandler)
    {
        _listHandler = listHandler;
        _byIdHandler = byIdHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<EventSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<EventSummaryDto>>> List(CancellationToken cancellationToken)
    {
        var events = await _listHandler.Handle(cancellationToken);
        return Ok(events);
    }

    [HttpGet("{id:guid}", Name = nameof(GetById))]
    [ProducesResponseType(typeof(EventDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventDetailDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var detail = await _byIdHandler.Handle(new GetEventByIdQuery(id), cancellationToken);
        return Ok(detail);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.OrganizerOnly)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        [FromBody] CreateEventCommand command,
        CancellationToken cancellationToken)
    {
        var id = await _createHandler.Handle(command, cancellationToken);
        return CreatedAtRoute(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.OrganizerOnly)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateEventBody body,
        CancellationToken cancellationToken)
    {
        var command = new UpdateEventCommand(
            id, body.Title, body.Subtitle, body.Description, body.IsVip, body.Date,
            body.Hero, body.Location, body.Registration, body.LayoutId);

        await _updateHandler.Handle(command, cancellationToken);
        return NoContent();
    }
}
