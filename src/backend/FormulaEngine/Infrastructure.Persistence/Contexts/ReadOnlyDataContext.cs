using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Contexts;

internal class ReadOnlyDataContext : DataContext
{
    public ReadOnlyDataContext(DbContextOptions<DataContext> dbContext) : base(dbContext)
    {
    }

    protected ReadOnlyDataContext(DbContextOptions options) : base(options)
    {
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => throw new InvalidOperationException("Unable to save changes in read-only context.");

    public override int SaveChanges() => throw new InvalidOperationException("Unable to save changes in read-only context.");
}
