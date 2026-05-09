namespace WhatIsOn.Domain.Entities;

public class Layout
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<LayoutComponent> Components { get; set; } = new List<LayoutComponent>();
}
