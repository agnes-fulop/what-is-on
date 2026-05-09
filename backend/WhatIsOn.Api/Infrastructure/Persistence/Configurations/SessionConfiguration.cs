using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatIsOn.Domain.Entities;

namespace WhatIsOn.Infrastructure.Persistence.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("Sessions");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title).IsRequired().HasMaxLength(256);
        builder.Property(s => s.Description).HasMaxLength(4000);
        builder.Property(s => s.From).IsRequired();
        builder.Property(s => s.To).IsRequired();
        builder.Property(s => s.Level).HasConversion<int>().IsRequired();
        builder.Property(s => s.Track).HasMaxLength(128);
        builder.Property(s => s.Room).HasMaxLength(128);
        builder.Property(s => s.SortOrder).IsRequired();

        builder.HasOne(s => s.Event)
            .WithMany(e => e.Sessions)
            .HasForeignKey(s => s.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Speaker)
            .WithMany(sp => sp.Sessions)
            .HasForeignKey(s => s.SpeakerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(s => s.EventId);
    }
}
