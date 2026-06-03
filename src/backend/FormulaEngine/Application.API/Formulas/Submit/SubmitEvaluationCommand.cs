using Application.API.Common;
using Domain.Expressions;

namespace Application.API.Formulas.Submit;

public sealed record SubmitEvaluationCommand(Guid UserId, Guid FormulaId, Guid EvaluationId, Dictionary<string, IValue> Arguments) : ICommand
{
    public const string Path = "/formulas/{formulaId:guid}/evaluations/{evaluationId:guid}/submit";
}
