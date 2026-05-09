using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatIsOn.Domain.Entities;

namespace WhatIsOn.Infrastructure.Persistence.Configurations;

public class LayoutComponentConfiguration : IEntityTypeConfiguration<LayoutComponent>
{
    public void Configure(EntityTypeBuilder<LayoutComponent> builder)
    {
        builder.ToTable("LayoutComponents");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.ComponentType)
            .HasConversion<int>()
            .IsRequired();

        // Polymorphic JSON payload — shape depends on ComponentType.
        // Stored as TEXT so any JSON object is acceptable.
        builder.Property(c => c.Data)
            .HasColumnType("TEXT")
            .IsRequired();

        builder.Property(c => c.SortOrder).IsRequired();

        // Self-referential adjacency list for the component tree.
        // Layout relationship is configured on the Layout side.
        builder.HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentComponentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.LayoutId);
        builder.HasIndex(c => c.ParentComponentId);
    }
}
