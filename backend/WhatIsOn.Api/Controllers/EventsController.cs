using Microsoft.AspNetCore.Mvc;
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

    public EventsController(GetEventListHandler listHandler, GetEventByIdHandler byIdHandler)
    {
        _listHandler = listHandler;
        _byIdHandler = byIdHandler;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<EventSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<EventSummaryDto>>> List(CancellationToken cancellationToken)
    {
        var events = await _listHandler.Handle(cancellationToken);
        return Ok(events);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventDetailDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var detail = await _byIdHandler.Handle(new GetEventByIdQuery(id), cancellationToken);
        return Ok(detail);
    }
}
