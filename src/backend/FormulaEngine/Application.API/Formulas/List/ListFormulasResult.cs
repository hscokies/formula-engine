namespace Application.API.Formulas.List;

public sealed record ListFormulasResult(IEnumerable<Formula> Items);

public sealed record Formula(Guid Id, string Name);
