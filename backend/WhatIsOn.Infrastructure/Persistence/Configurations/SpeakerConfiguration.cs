using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatIsOn.Domain.Entities;

namespace WhatIsOn.Infrastructure.Persistence.Configurations;

public class SpeakerConfiguration : IEntityTypeConfiguration<Speaker>
{
    public void Configure(EntityTypeBuilder<Speaker> builder)
    {
        builder.ToTable("Speakers");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(256);
        builder.Property(s => s.Title).HasMaxLength(256);
        builder.Property(s => s.Bio).HasMaxLength(4000);
        builder.Property(s => s.Image).HasMaxLength(1024);
    }
}
