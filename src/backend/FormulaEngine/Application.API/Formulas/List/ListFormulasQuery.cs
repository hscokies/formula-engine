using Application.API.Common;

namespace Application.API.Formulas.List;

public sealed record ListFormulasQuery(Guid UserId, string? Search) : IQuery<ListFormulasResult>
{
    public const string Path = "/formulas";
}
