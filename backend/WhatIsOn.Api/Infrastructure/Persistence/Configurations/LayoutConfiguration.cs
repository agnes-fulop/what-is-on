using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatIsOn.Domain.Entities;

namespace WhatIsOn.Infrastructure.Persistence.Configurations;

public class LayoutConfiguration : IEntityTypeConfiguration<Layout>
{
    public void Configure(EntityTypeBuilder<Layout> builder)
    {
        builder.ToTable("Layouts");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name).HasMaxLength(128);

        builder.HasMany(l => l.Components)
            .WithOne(c => c.Layout)
            .HasForeignKey(c => c.LayoutId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
