using WhatIsOn.Domain.Enums;

namespace WhatIsOn.Application.Layouts.DTOs;

/// <summary>
/// One node in the rendered component tree. <see cref="Data"/> is typed as
/// <c>object</c> so the runtime-type's properties are emitted by the JSON
/// serializer; the client uses <see cref="Type"/> to decode it.
/// </summary>
public record LayoutComponentDto(
    Guid Id,
    ComponentType Type,
    object Data,
    IReadOnlyList<LayoutComponentDto> Children);
