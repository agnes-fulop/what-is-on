using WhatIsOn.Application.Events.DTOs;

namespace WhatIsOn.Application.Layouts.DTOs;

/// <summary>
/// Marker base for the polymorphic shape of <see cref="LayoutComponentDto.Data"/>.
/// Records below correspond one-to-one with <see cref="WhatIsOn.Domain.Enums.ComponentType"/>.
/// At the API boundary the property is typed as <c>object</c> so System.Text.Json
/// emits the runtime-type's properties without needing a $type discriminator —
/// the parent component's <c>type</c> field already tells the client how to read this.
/// </summary>
public abstract record ComponentData;

public sealed record SectionData(string Direction) : ComponentData;

public sealed record HeadingData(string Text, string Level) : ComponentData;

public sealed record ParagraphData(string Text) : ComponentData;

public sealed record SpeakerListData(string Title) : ComponentData;

public sealed record SpeakerCardData(SpeakerDto? Speaker) : ComponentData;

public sealed record SessionScheduleData(string Title) : ComponentData;

public sealed record SessionCardData(SessionDto? Session) : ComponentData;
