using WhatIsOn.Domain.Enums;

namespace WhatIsOn.Domain.Entities;

public class Session
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;

    public Guid? SpeakerId { get; set; }
    public Speaker? Speaker { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public SessionLevel Level { get; set; }
    public string Track { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}
