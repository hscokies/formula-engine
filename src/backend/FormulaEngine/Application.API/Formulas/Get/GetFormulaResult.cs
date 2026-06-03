using Application.API.Formulas.Configure;

namespace Application.API.Formulas.Get;

public sealed record GetFormulaResult(Guid Id, string Name, Dictionary<string, FieldConfiguration> Fields);
