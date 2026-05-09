using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WhatIsOn.Domain.Entities;
using WhatIsOn.Domain.Interfaces;

namespace WhatIsOn.Infrastructure.Persistence;

public class WhatIsOnDbContext : DbContext, IUnitOfWork
{
    public WhatIsOnDbContext(DbContextOptions<WhatIsOnDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Speaker> Speakers => Set<Speaker>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Layout> Layouts => Set<Layout>();
    public DbSet<LayoutComponent> LayoutComponents => Set<LayoutComponent>();
    public DbSet<Registration> Registrations => Set<Registration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
