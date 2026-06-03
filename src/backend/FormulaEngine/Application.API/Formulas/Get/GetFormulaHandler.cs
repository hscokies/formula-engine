using Application.API.Common;
using Application.API.Formulas.Configure;
using Domain.Common;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Application.API.Formulas.Get;

public sealed class GetFormulaHandler(IReadOnlyDataContext dataContext)
    : IQueryHandler<GetFormulaQuery, GetFormulaResult>
{
    public async Task<Result<GetFormulaResult>> Handle(GetFormulaQuery query, CancellationToken cancellationToken)
    {
        var formula = await dataContext.Formulas
            .Where(x => x.Id == query.Id)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.FieldsConfiguration
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (formula is null)
        {
            return FormulaErrors.NotFound;
        }

        return new GetFormulaResult(
            formula.Id,
            formula.Name,
            formula.FieldsConfiguration.ToDictionary(
                f => f.InternalName,
                f => new FieldConfiguration(f.Label ?? f.InternalName, f.Type)
            )
        );
    }
}
