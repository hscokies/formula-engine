using Application.API.Common;
using Domain.Common;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Application.API.Formulas.List;

public sealed class ListFormulasHandler(IReadOnlyDataContext dataContext) : IQueryHandler<ListFormulasQuery, ListFormulasResult>
{
    public async Task<Result<ListFormulasResult>> Handle(ListFormulasQuery query, CancellationToken cancellationToken)
    {
        
        var queryable = dataContext.Formulas.Where(x => x.OwnerId == query.UserId);

        var searchNormalized = query.Search?.Trim().ToUpperInvariant();
        if (!string.IsNullOrWhiteSpace(searchNormalized))
        {
            queryable = queryable.Where(x => EF.Functions.Like(x.NameNormalized, $"{searchNormalized}%"));
        }

        var items = await queryable.Select(x => new Formula(x.Id, x.Name)).ToListAsync(cancellationToken);
        return new ListFormulasResult(items);
    }
}
