using WhatIsOn.Domain.Enums;

namespace WhatIsOn.Domain.Entities;

public class LayoutComponent
{
    public Guid Id { get; set; }

    public Guid LayoutId { get; set; }
    public Layout Layout { get; set; } = null!;

    public Guid? ParentComponentId { get; set; }
    public LayoutComponent? Parent { get; set; }

    public ComponentType ComponentType { get; set; }

    /// <summary>
    /// JSON payload whose shape depends on <see cref="ComponentType"/>.
    /// Stored as TEXT; deserialized into a typed DTO in the Application layer.
    /// </summary>
    public string Data { get; set; } = "{}";

    public int SortOrder { get; set; }

    public ICollection<LayoutComponent> Children { get; set; } = new List<LayoutComponent>();
}
