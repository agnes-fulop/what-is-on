using WhatIsOn.Domain.ValueObjects;

namespace WhatIsOn.Domain.Entities;

public class Event
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsVip { get; set; }
    public DateOnly Date { get; set; }

    public Hero Hero { get; set; } = new();
    public Location Location { get; set; } = new();
    public RegistrationInfo Registration { get; set; } = new();

    public Guid OrganizerId { get; set; }
    public User Organizer { get; set; } = null!;

    public Guid? LayoutId { get; set; }
    public Layout? Layout { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Session> Sessions { get; set; } = new List<Session>();
    public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
}
