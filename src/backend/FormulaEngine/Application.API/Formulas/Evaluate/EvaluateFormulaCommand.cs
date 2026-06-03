using Application.API.Common;
using Domain.Expressions;

namespace Application.API.Formulas.Evaluate;

public sealed record EvaluateFormulaCommand(Guid Id, Dictionary<string, IValue> Arguments)
    : ICommand<EvaluateFormulaResult>
{
    public const string Path = "/formulas/{id:guid}/evaluate";
}
