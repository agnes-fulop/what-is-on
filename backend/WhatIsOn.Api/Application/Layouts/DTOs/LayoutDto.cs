namespace WhatIsOn.Application.Layouts.DTOs;

public record LayoutDto(Guid Id, IReadOnlyList<LayoutComponentDto> Components);
