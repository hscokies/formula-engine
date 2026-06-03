using Application.API.Common;
using Domain.Formulas;

namespace Application.API.Formulas.Configure;

public sealed record ConfigureFormulaCommand(Guid Id, string Name, Dictionary<string, FieldConfiguration> Fields) : ICommand
{
    public const string Path = "/formulas/{id:guid}/configure";
}

public sealed record FieldConfiguration(string Label, FieldType Type);
