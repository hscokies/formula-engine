using Application.API.Common;

namespace Application.API.Submissions.List;

public sealed record ListSubmissionsQuery(Guid FormulaId) : IQuery<ListSubmissionsResult>
{
    public const string Path = "/formulas/{id:guid}/submissions";
}
