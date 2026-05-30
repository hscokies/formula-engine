using Domain.Formulas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Persistence.Contexts;

public interface IReadOnlyDataContext
{
    DatabaseFacade Database { get; }
    DbSet<Formula> Formulas { get; }
}

public interface IDataContext : IReadOnlyDataContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
