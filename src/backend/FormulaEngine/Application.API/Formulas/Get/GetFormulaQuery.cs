using Application.API.Common;

namespace Application.API.Formulas.Get;

public sealed record GetFormulaQuery(Guid Id) : IQuery<GetFormulaResult>
{
    public const string Path = "/formulas/{id:guid}";
}
