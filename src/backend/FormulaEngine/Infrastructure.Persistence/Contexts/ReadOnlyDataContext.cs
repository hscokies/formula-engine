using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Contexts;

public sealed class ReadOnlyDataContext : DataContext
{
    public ReadOnlyDataContext(DbContextOptions<DataContext> dbContext) : base(dbContext)
    {
    }

    public ReadOnlyDataContext(DbContextOptions options) : base(options)
    {
    }

    public override int SaveChanges() =>
        throw new InvalidOperationException("unable to save changes: Read-only context");

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new()) =>
        throw new InvalidOperationException("unable to save changes: Read-only context");

    public override int SaveChanges(bool acceptAllChangesOnSuccess) =>
        throw new InvalidOperationException("unable to save changes: Read-only context");
}
