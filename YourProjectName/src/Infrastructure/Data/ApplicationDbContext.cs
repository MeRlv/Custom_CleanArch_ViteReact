using System.Reflection;
using YourProjectName.Application.Common.Interfaces;
using YourProjectName.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace YourProjectName.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder
            .HasSequence<int>("SEQ_USER")
            .StartsAt(1)
            .IncrementsBy(1);
    }
}
