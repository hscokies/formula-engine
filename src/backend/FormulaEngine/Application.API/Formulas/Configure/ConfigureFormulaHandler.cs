using System.Text.Json;
using Application.API.Common;
using Domain.Common;
using Domain.Expressions;
using Infrastructure.Persistence.Contexts;

namespace Application.API.Formulas.Configure;

public sealed class ConfigureFormulaHandler(IDataContext dataContext) : ICommandHandler<ConfigureFormulaCommand>
{
    public async Task<Result> Handle(ConfigureFormulaCommand command, CancellationToken cancellationToken)
    {
        var formula = await dataContext.Formulas.FindAsync([command.Id], cancellationToken);
        if (formula is null)
        {
            return FormulaErrors.NotFound;
        }

        
        foreach (var (internalName, configuration) in command.Fields)
        {
            var field = formula.FieldsConfiguration.FirstOrDefault(x => x.InternalName == internalName);
            if (field is null)
            {
                return FormulaErrors.FieldNotFound;
            }
            
            field.Label = configuration.Label;
            field.Type = configuration.Type;
        }

        dataContext.Formulas.Update(formula);
        await dataContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
