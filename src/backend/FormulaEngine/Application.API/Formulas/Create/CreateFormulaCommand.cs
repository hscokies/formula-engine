using Application.API.Common;

namespace Application.API.Formulas.Create;

public sealed record CreateFormulaCommand(Guid UserId, string Name, string Expression) : ICommand<CreateFormulaResult>
{
    public const string Path = "/formulas/create";
}
