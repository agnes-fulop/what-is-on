using System.Text.Json;
using WhatIsOn.Domain.Entities;
using WhatIsOn.Domain.Enums;

namespace WhatIsOn.Infrastructure.Persistence.Seed;

internal static class LayoutsSeed
{
    public static IReadOnlyList<Layout> Build()
    {
        return new[]
        {
            BuildAwsLayout(),
            BuildVipLayout(),
            BuildFrontendLayout()
        };
    }

    /// <summary>
    /// Mirrors layout.mock.json: an "About" section with a nested sub-section,
    /// a SpeakerList of multiple cards, and a SessionSchedule of multiple cards.
    /// Exercises every component type plus the nested-Section case.
    /// </summary>
    private static Layout BuildAwsLayout()
    {
        var layoutId = SeedData.Layouts.AwsLayout;
        var components = new List<LayoutComponent>();

        var aboutSection = AddRoot(components, layoutId, ComponentType.Section,
            sortOrder: 0, data: new { direction = "col" });

        AddChild(components, layoutId, aboutSection.Id, ComponentType.Heading,
            sortOrder: 0, data: new { text = "About the Summit", level = "h2" });

        AddChild(components, layoutId, aboutSection.Id, ComponentType.Paragraph,
            sortOrder: 1, data: new
            {
                text = "The AWS Cloud Innovators Summit is a premier event for engineers, " +
                       "architects, and decision makers who build on the AWS platform."
            });

        var nestedSection = AddChild(components, layoutId, aboutSection.Id, ComponentType.Section,
            sortOrder: 2, data: new { direction = "col" });

        AddChild(components, layoutId, nestedSection.Id, ComponentType.Heading,
            sortOrder: 0, data: new { text = "What to expect?", level = "h3" });

        AddChild(components, layoutId, nestedSection.Id, ComponentType.Paragraph,
            sortOrder: 1, data: new
            {
                text = "- A mix of general and technical talks and discussions.\n" +
                       "- Networking opportunities with the AWS community.\n" +
                       "- Drinks, snacks, and an after-party."
            });

        var speakerList = AddRoot(components, layoutId, ComponentType.SpeakerList,
            sortOrder: 1, data: new { title = "Featured Speakers" });

        AddSpeakerCard(components, layoutId, speakerList.Id, sortOrder: 0, SeedData.Speakers.JaneDoe);
        AddSpeakerCard(components, layoutId, speakerList.Id, sortOrder: 1, SeedData.Speakers.AishaKhan);
        AddSpeakerCard(components, layoutId, speakerList.Id, sortOrder: 2, SeedData.Speakers.MarcusLee);
        AddSpeakerCard(components, layoutId, speakerList.Id, sortOrder: 3, SeedData.Speakers.ElenaCosta);

        var schedule = AddRoot(components, layoutId, ComponentType.SessionSchedule,
            sortOrder: 2, data: new { title = "Session Schedule" });

        AddSessionCard(components, layoutId, schedule.Id, sortOrder: 0, SeedData.Sessions.AwsServerless);
        AddSessionCard(components, layoutId, schedule.Id, sortOrder: 1, SeedData.Sessions.AwsMl);
        AddSessionCard(components, layoutId, schedule.Id, sortOrder: 2, SeedData.Sessions.AwsDevRel);
        AddSessionCard(components, layoutId, schedule.Id, sortOrder: 3, SeedData.Sessions.AwsSecurity);

        return new Layout { Id = layoutId, Name = "AWS Summit Layout", Components = components };
    }

    /// <summary>
    /// VIP layout: introduction section, schedule with two sessions, speaker
    /// list with two speakers.
    /// </summary>
    private static Layout BuildVipLayout()
    {
        var layoutId = SeedData.Layouts.VipLayout;
        var components = new List<LayoutComponent>();

        var intro = AddRoot(components, layoutId, ComponentType.Section,
            sortOrder: 0, data: new { direction = "col" });

        AddChild(components, layoutId, intro.Id, ComponentType.Heading,
            sortOrder: 0, data: new { text = "VIP Members Only", level = "h2" });

        AddChild(components, layoutId, intro.Id, ComponentType.Paragraph,
            sortOrder: 1, data: new
            {
                text = "A closed-door day for our VIP community. Strategic briefings, " +
                       "candid panels, and structured networking over dinner."
            });

        var schedule = AddRoot(components, layoutId, ComponentType.SessionSchedule,
            sortOrder: 1, data: new { title = "Schedule" });

        AddSessionCard(components, layoutId, schedule.Id, sortOrder: 0, SeedData.Sessions.VipRoadmap);
        AddSessionCard(components, layoutId, schedule.Id, sortOrder: 1, SeedData.Sessions.VipMa);

        var speakers = AddRoot(components, layoutId, ComponentType.SpeakerList,
            sortOrder: 2, data: new { title = "Featured Speakers" });

        AddSpeakerCard(components, layoutId, speakers.Id, sortOrder: 0, SeedData.Speakers.TomasBrandt);
        AddSpeakerCard(components, layoutId, speakers.Id, sortOrder: 1, SeedData.Speakers.ElenaCosta);

        return new Layout { Id = layoutId, Name = "VIP Layout", Components = components };
    }

    /// <summary>
    /// Minimal layout: introduction section + a single-speaker SpeakerList.
    /// Useful as a contrast to the rich layouts.
    /// </summary>
    private static Layout BuildFrontendLayout()
    {
        var layoutId = SeedData.Layouts.FrontendLayout;
        var components = new List<LayoutComponent>();

        var intro = AddRoot(components, layoutId, ComponentType.Section,
            sortOrder: 0, data: new { direction = "col" });

        AddChild(components, layoutId, intro.Id, ComponentType.Heading,
            sortOrder: 0, data: new { text = "About the Conference", level = "h2" });

        AddChild(components, layoutId, intro.Id, ComponentType.Paragraph,
            sortOrder: 1, data: new
            {
                text = "Three days of frontend talks — from frameworks to platform features."
            });

        var speakers = AddRoot(components, layoutId, ComponentType.SpeakerList,
            sortOrder: 1, data: new { title = "Speakers" });

        AddSpeakerCard(components, layoutId, speakers.Id, sortOrder: 0, SeedData.Speakers.MarcusLee);

        return new Layout { Id = layoutId, Name = "Frontend Layout", Components = components };
    }

    private static LayoutComponent AddRoot(
        List<LayoutComponent> components, Guid layoutId, ComponentType type, int sortOrder, object data)
    {
        var component = new LayoutComponent
        {
            Id = Guid.NewGuid(),
            LayoutId = layoutId,
            ParentComponentId = null,
            ComponentType = type,
            Data = JsonSerializer.Serialize(data),
            SortOrder = sortOrder
        };
        components.Add(component);
        return component;
    }

    private static LayoutComponent AddChild(
        List<LayoutComponent> components, Guid layoutId, Guid parentId,
        ComponentType type, int sortOrder, object data)
    {
        var component = new LayoutComponent
        {
            Id = Guid.NewGuid(),
            LayoutId = layoutId,
            ParentComponentId = parentId,
            ComponentType = type,
            Data = JsonSerializer.Serialize(data),
            SortOrder = sortOrder
        };
        components.Add(component);
        return component;
    }

    private static void AddSpeakerCard(
        List<LayoutComponent> components, Guid layoutId, Guid parentId, int sortOrder, Guid speakerId)
    {
        AddChild(components, layoutId, parentId, ComponentType.SpeakerCard,
            sortOrder, data: new { speakerId });
    }

    private static void AddSessionCard(
        List<LayoutComponent> components, Guid layoutId, Guid parentId, int sortOrder, Guid sessionId)
    {
        AddChild(components, layoutId, parentId, ComponentType.SessionCard,
            sortOrder, data: new { sessionId });
    }
}
