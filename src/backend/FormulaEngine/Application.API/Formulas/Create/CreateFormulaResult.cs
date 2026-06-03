namespace Application.API.Formulas.Create;

public sealed record CreateFormulaResult(Guid Id, IEnumerable<string> Fields);
