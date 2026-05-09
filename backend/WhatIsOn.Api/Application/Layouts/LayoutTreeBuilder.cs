using System.Text.Json;
using WhatIsOn.Application.Events.DTOs;
using WhatIsOn.Application.Layouts.DTOs;
using WhatIsOn.Domain.Entities;
using WhatIsOn.Domain.Enums;

namespace WhatIsOn.Application.Layouts;

/// <summary>
/// Reconstructs the nested layout component tree from the flat adjacency-list
/// rows returned by the database, enriching SpeakerCard and SessionCard nodes
/// with their referenced entities. All enrichment lookups are in-memory after
/// the caller has batch-loaded the relevant speakers and sessions, so there
/// are no N+1 queries inside this builder.
/// </summary>
public class LayoutTreeBuilder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Returns every speaker id referenced by SpeakerCard components in a flat
    /// list. Used by the read-path handler to batch-load the speakers in a
    /// single query before invoking <see cref="Build"/>.
    /// </summary>
    public static IReadOnlyList<Guid> ExtractSpeakerIds(IEnumerable<LayoutComponent> components)
    {
        var ids = new HashSet<Guid>();
        foreach (var component in components)
        {
            if (component.ComponentType != ComponentType.SpeakerCard) continue;
            var stored = Deserialize<StoredSpeakerCardData>(component.Data);
            if (stored is not null) ids.Add(stored.SpeakerId);
        }
        return ids.ToList();
    }

    public LayoutDto Build(
        Layout layout,
        IReadOnlyDictionary<Guid, Speaker> speakersById,
        IReadOnlyDictionary<Guid, Session> sessionsById)
    {
        var byParent = layout.Components
            .GroupBy(c => c.ParentComponentId)
            .ToDictionary(g => g.Key ?? Guid.Empty, g => g.OrderBy(c => c.SortOrder).ToList());

        var roots = BuildChildren(parentId: null, byParent, speakersById, sessionsById);
        return new LayoutDto(layout.Id, roots);
    }

    private IReadOnlyList<LayoutComponentDto> BuildChildren(
        Guid? parentId,
        IReadOnlyDictionary<Guid, List<LayoutComponent>> byParent,
        IReadOnlyDictionary<Guid, Speaker> speakersById,
        IReadOnlyDictionary<Guid, Session> sessionsById)
    {
        var lookupKey = parentId ?? Guid.Empty;
        if (!byParent.TryGetValue(lookupKey, out var siblings))
        {
            return Array.Empty<LayoutComponentDto>();
        }

        var result = new List<LayoutComponentDto>(siblings.Count);
        foreach (var component in siblings)
        {
            var data = MaterializeData(component, speakersById, sessionsById);
            var children = BuildChildren(component.Id, byParent, speakersById, sessionsById);
            result.Add(new LayoutComponentDto(component.Id, component.ComponentType, data, children));
        }
        return result;
    }

    private static object MaterializeData(
        LayoutComponent component,
        IReadOnlyDictionary<Guid, Speaker> speakersById,
        IReadOnlyDictionary<Guid, Session> sessionsById)
    {
        return component.ComponentType switch
        {
            ComponentType.Section => Deserialize<StoredSectionData>(component.Data) is { } s
                ? new SectionData(s.Direction)
                : new SectionData("col"),

            ComponentType.Heading => Deserialize<StoredHeadingData>(component.Data) is { } h
                ? new HeadingData(h.Text, h.Level)
                : new HeadingData(string.Empty, "h2"),

            ComponentType.Paragraph => Deserialize<StoredParagraphData>(component.Data) is { } p
                ? new ParagraphData(p.Text)
                : new ParagraphData(string.Empty),

            ComponentType.SpeakerList => new SpeakerListData(Deserialize<StoredTitleData>(component.Data)?.Title ?? string.Empty),

            ComponentType.SessionSchedule => new SessionScheduleData(Deserialize<StoredTitleData>(component.Data)?.Title ?? string.Empty),

            ComponentType.SpeakerCard => EnrichSpeakerCard(component, speakersById),

            ComponentType.SessionCard => EnrichSessionCard(component, sessionsById),

            _ => new { }
        };
    }

    private static SpeakerCardData EnrichSpeakerCard(
        LayoutComponent component,
        IReadOnlyDictionary<Guid, Speaker> speakersById)
    {
        var stored = Deserialize<StoredSpeakerCardData>(component.Data);
        if (stored is null || !speakersById.TryGetValue(stored.SpeakerId, out var speaker))
        {
            return new SpeakerCardData(Speaker: null);
        }

        return new SpeakerCardData(new SpeakerDto(
            speaker.Id, speaker.Name, speaker.Title, speaker.Bio, speaker.Image));
    }

    private static SessionCardData EnrichSessionCard(
        LayoutComponent component,
        IReadOnlyDictionary<Guid, Session> sessionsById)
    {
        var stored = Deserialize<StoredSessionCardData>(component.Data);
        if (stored is null || !sessionsById.TryGetValue(stored.SessionId, out var session))
        {
            return new SessionCardData(Session: null);
        }

        var speakerDto = session.Speaker is null
            ? null
            : new SpeakerDto(session.Speaker.Id, session.Speaker.Name, session.Speaker.Title,
                             session.Speaker.Bio, session.Speaker.Image);

        return new SessionCardData(new SessionDto(
            session.Id, session.Title, session.Description, session.From, session.To,
            session.Level, session.Track, session.Room, speakerDto));
    }

    private static T? Deserialize<T>(string json) where T : class
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private record StoredSectionData(string Direction);
    private record StoredHeadingData(string Text, string Level);
    private record StoredParagraphData(string Text);
    private record StoredTitleData(string Title);
    private record StoredSpeakerCardData(Guid SpeakerId);
    private record StoredSessionCardData(Guid SessionId);
}
