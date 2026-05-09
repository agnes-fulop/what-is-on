using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhatIsOn.Domain.Entities;

namespace WhatIsOn.Infrastructure.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title).IsRequired().HasMaxLength(256);
        builder.Property(e => e.Subtitle).HasMaxLength(512);
        builder.Property(e => e.Description).HasMaxLength(4000);
        builder.Property(e => e.IsVip).IsRequired();
        builder.Property(e => e.Date).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt).IsRequired();

        builder.OwnsOne(e => e.Hero, hero =>
        {
            hero.Property(h => h.Image).HasColumnName("HeroImage").HasMaxLength(1024);
            hero.Property(h => h.CtaText).HasColumnName("HeroCtaText").HasMaxLength(128);
        });

        builder.OwnsOne(e => e.Location, location =>
        {
            location.Property(l => l.City).HasColumnName("City").HasMaxLength(128);
            location.Property(l => l.Venue).HasColumnName("Venue").HasMaxLength(256);
            location.Property(l => l.Address).HasColumnName("Address").HasMaxLength(512);
        });

        builder.OwnsOne(e => e.Registration, registration =>
        {
            registration.Property(r => r.OpenDate).HasColumnName("RegistrationOpenDate");
            registration.Property(r => r.CloseDate).HasColumnName("RegistrationCloseDate");
            registration.Property(r => r.Fee).HasColumnName("RegistrationFee").HasPrecision(10, 2);
            registration.Property(r => r.EarlyBirdDiscount).HasColumnName("EarlyBirdDiscount").HasPrecision(10, 2);
        });

        builder.HasOne(e => e.Organizer)
            .WithMany(u => u.OrganizedEvents)
            .HasForeignKey(e => e.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Layout)
            .WithMany()
            .HasForeignKey(e => e.LayoutId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.Date);
        builder.HasIndex(e => e.IsVip);
    }
}
