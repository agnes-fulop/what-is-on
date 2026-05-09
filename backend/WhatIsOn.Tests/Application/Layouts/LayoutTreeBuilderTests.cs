using System.Text.Json;
using Shouldly;
using WhatIsOn.Application.Events.DTOs;
using WhatIsOn.Application.Layouts;
using WhatIsOn.Application.Layouts.DTOs;
using WhatIsOn.Domain.Entities;
using WhatIsOn.Domain.Enums;

namespace WhatIsOn.Tests.Application.Layouts;

public class LayoutTreeBuilderTests
{
    private readonly LayoutTreeBuilder _sut = new();

    [Fact]
    public void Build_EmptyLayout_ReturnsLayoutWithNoComponents()
    {
        var layout = new Layout { Id = Guid.NewGuid(), Components = new List<LayoutComponent>() };

        var result = _sut.Build(layout, EmptySpeakers, EmptySessions);

        result.Id.ShouldBe(layout.Id);
        result.Components.ShouldBeEmpty();
    }

    [Fact]
    public void Build_ReturnsRootsOnlyAtTopLevel()
    {
        var layoutId = Guid.NewGuid();
        var root1 = MakeComponent(layoutId, parent: null, ComponentType.Heading,
            new { text = "First", level = "h1" }, sortOrder: 0);
        var root2 = MakeComponent(layoutId, parent: null, ComponentType.Heading,
            new { text = "Second", level = "h1" }, sortOrder: 1);

        var result = _sut.Build(LayoutOf(layoutId, root1, root2), EmptySpeakers, EmptySessions);

        result.Components.Count.ShouldBe(2);
        result.Components[0].Id.ShouldBe(root1.Id);
        result.Components[1].Id.ShouldBe(root2.Id);
    }

    [Fact]
    public void Build_NestedSections_ProducesHierarchy()
    {
        var layoutId = Guid.NewGuid();
        var outer = MakeComponent(layoutId, null, ComponentType.Section, new { direction = "col" }, 0);
        var inner = MakeComponent(layoutId, outer.Id, ComponentType.Section, new { direction = "row" }, 0);
        var leaf = MakeComponent(layoutId, inner.Id, ComponentType.Paragraph, new { text = "deep" }, 0);

        var result = _sut.Build(LayoutOf(layoutId, outer, inner, leaf), EmptySpeakers, EmptySessions);

        result.Components.ShouldHaveSingleItem();
        var outerNode = result.Components[0];
        outerNode.Children.ShouldHaveSingleItem();
        var innerNode = outerNode.Children[0];
        innerNode.Type.ShouldBe(ComponentType.Section);
        innerNode.Children[0].Type.ShouldBe(ComponentType.Paragraph);
    }

    [Fact]
    public void Build_RespectsSortOrderAmongSiblings()
    {
        var layoutId = Guid.NewGuid();
        // Insert in reverse sort order; the builder must order by SortOrder.
        var third = MakeComponent(layoutId, null, ComponentType.Paragraph, new { text = "C" }, 2);
        var first = MakeComponent(layoutId, null, ComponentType.Paragraph, new { text = "A" }, 0);
        var second = MakeComponent(layoutId, null, ComponentType.Paragraph, new { text = "B" }, 1);

        var result = _sut.Build(LayoutOf(layoutId, third, first, second), EmptySpeakers, EmptySessions);

        result.Components.Select(c => ((ParagraphData)c.Data).Text)
            .ShouldBe(new[] { "A", "B", "C" });
    }

    [Fact]
    public void Build_SectionData_ReadsDirection()
    {
        var layoutId = Guid.NewGuid();
        var section = MakeComponent(layoutId, null, ComponentType.Section, new { direction = "row" }, 0);

        var result = _sut.Build(LayoutOf(layoutId, section), EmptySpeakers, EmptySessions);

        var data = result.Components[0].Data.ShouldBeOfType<SectionData>();
        data.Direction.ShouldBe("row");
    }

    [Fact]
    public void Build_HeadingData_ReadsTextAndLevel()
    {
        var layoutId = Guid.NewGuid();
        var heading = MakeComponent(layoutId, null, ComponentType.Heading,
            new { text = "About", level = "h2" }, 0);

        var result = _sut.Build(LayoutOf(layoutId, heading), EmptySpeakers, EmptySessions);

        var data = result.Components[0].Data.ShouldBeOfType<HeadingData>();
        data.Text.ShouldBe("About");
        data.Level.ShouldBe("h2");
    }

    [Fact]
    public void Build_SpeakerCard_EnrichesFromSpeakerDictionary()
    {
        var speaker = SampleSpeaker();
        var layoutId = Guid.NewGuid();
        var card = MakeComponent(layoutId, null, ComponentType.SpeakerCard,
            new { speakerId = speaker.Id }, 0);

        var speakers = new Dictionary<Guid, Speaker> { [speaker.Id] = speaker };
        var result = _sut.Build(LayoutOf(layoutId, card), speakers, EmptySessions);

        var data = result.Components[0].Data.ShouldBeOfType<SpeakerCardData>();
        data.Speaker.ShouldNotBeNull();
        data.Speaker!.Id.ShouldBe(speaker.Id);
        data.Speaker.Name.ShouldBe(speaker.Name);
    }

    [Fact]
    public void Build_SpeakerCardWithUnknownId_ReturnsNullSpeaker()
    {
        var layoutId = Guid.NewGuid();
        var card = MakeComponent(layoutId, null, ComponentType.SpeakerCard,
            new { speakerId = Guid.NewGuid() }, 0);

        var result = _sut.Build(LayoutOf(layoutId, card), EmptySpeakers, EmptySessions);

        var data = result.Components[0].Data.ShouldBeOfType<SpeakerCardData>();
        data.Speaker.ShouldBeNull();
    }

    [Fact]
    public void Build_SessionCard_EnrichesIncludingSpeaker()
    {
        var speaker = SampleSpeaker();
        var session = SampleSession(speaker);
        var layoutId = Guid.NewGuid();
        var card = MakeComponent(layoutId, null, ComponentType.SessionCard,
            new { sessionId = session.Id }, 0);

        var sessions = new Dictionary<Guid, Session> { [session.Id] = session };
        var result = _sut.Build(LayoutOf(layoutId, card), EmptySpeakers, sessions);

        var data = result.Components[0].Data.ShouldBeOfType<SessionCardData>();
        data.Session.ShouldNotBeNull();
        data.Session!.Id.ShouldBe(session.Id);
        data.Session.Speaker.ShouldNotBeNull();
        data.Session.Speaker!.Id.ShouldBe(speaker.Id);
    }

    [Fact]
    public void Build_SessionCardWithUnknownId_ReturnsNullSession()
    {
        var layoutId = Guid.NewGuid();
        var card = MakeComponent(layoutId, null, ComponentType.SessionCard,
            new { sessionId = Guid.NewGuid() }, 0);

        var result = _sut.Build(LayoutOf(layoutId, card), EmptySpeakers, EmptySessions);

        var data = result.Components[0].Data.ShouldBeOfType<SessionCardData>();
        data.Session.ShouldBeNull();
    }

    [Fact]
    public void ExtractSpeakerIds_ReturnsUniqueIdsFromSpeakerCardsOnly()
    {
        var speaker1 = Guid.NewGuid();
        var speaker2 = Guid.NewGuid();
        var components = new[]
        {
            MakeComponent(Guid.NewGuid(), null, ComponentType.SpeakerCard, new { speakerId = speaker1 }, 0),
            MakeComponent(Guid.NewGuid(), null, ComponentType.SpeakerCard, new { speakerId = speaker2 }, 1),
            MakeComponent(Guid.NewGuid(), null, ComponentType.SpeakerCard, new { speakerId = speaker1 }, 2),
            MakeComponent(Guid.NewGuid(), null, ComponentType.SessionCard, new { sessionId = Guid.NewGuid() }, 3),
            MakeComponent(Guid.NewGuid(), null, ComponentType.Paragraph, new { text = "x" }, 4),
        };

        var ids = LayoutTreeBuilder.ExtractSpeakerIds(components);

        ids.Count.ShouldBe(2);
        ids.ShouldContain(speaker1);
        ids.ShouldContain(speaker2);
    }

    [Fact]
    public void ExtractSpeakerIds_EmptyInput_ReturnsEmpty()
    {
        LayoutTreeBuilder.ExtractSpeakerIds(Array.Empty<LayoutComponent>()).ShouldBeEmpty();
    }

    // --- helpers ---

    private static readonly IReadOnlyDictionary<Guid, Speaker> EmptySpeakers =
        new Dictionary<Guid, Speaker>();

    private static readonly IReadOnlyDictionary<Guid, Session> EmptySessions =
        new Dictionary<Guid, Session>();

    private static LayoutComponent MakeComponent(
        Guid layoutId, Guid? parent, ComponentType type, object data, int sortOrder) => new()
    {
        Id = Guid.NewGuid(),
        LayoutId = layoutId,
        ParentComponentId = parent,
        ComponentType = type,
        Data = JsonSerializer.Serialize(data),
        SortOrder = sortOrder,
    };

    private static Layout LayoutOf(Guid id, params LayoutComponent[] components) => new()
    {
        Id = id,
        Components = components.ToList(),
    };

    private static Speaker SampleSpeaker() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Dr. Sample",
        Title = "Test Speaker",
        Bio = "bio",
        Image = "img.jpg",
    };

    private static Session SampleSession(Speaker speaker) => new()
    {
        Id = Guid.NewGuid(),
        Title = "Sample Session",
        Description = "desc",
        From = new DateTime(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc),
        To = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc),
        Level = SessionLevel.Beginner,
        Track = "Track",
        Room = "Room",
        SortOrder = 0,
        SpeakerId = speaker.Id,
        Speaker = speaker,
    };
}
