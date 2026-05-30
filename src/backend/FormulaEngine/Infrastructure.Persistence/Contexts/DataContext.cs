using Domain.Formulas;
using Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Contexts;

public class DataContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IDataContext
{
    public DataContext(DbContextOptions<DataContext> dbContext)
        : base(dbContext)
    {
    }

    protected DataContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Formula> Formulas { get; init; }
    
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
    }
}
