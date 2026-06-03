using Domain.Common;
using Domain.Expressions;
using Domain.Formulas;
using NLog;

namespace Application.API.Formulas;

internal static class EvaluationScopeFactory
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    
    public static Result<Dictionary<string, object>> Create(Formula formula, Dictionary<string, IValue> arguments)
    {
        Dictionary<string, object> scope = new();
        
        foreach (var (internalName, operand) in arguments)
        {
            var fieldConfiguration = formula.FieldsConfiguration.FirstOrDefault(x => x.InternalName == internalName);
            if (fieldConfiguration is null)
            {
                Logger.Warn("Encountered unknown formula field: {FieldName}", internalName);
                continue;
            }

            
            switch (fieldConfiguration.Type)
            {
                case FieldType.Decimal when operand.TryGet<decimal>(out var decimalValue):
                    scope.Add(internalName, decimalValue);
                    break;
                case FieldType.Integer when operand.TryGet<int>(out var intValue):
                    scope.Add(internalName, intValue);
                    break;
                default:
                    return FormulaErrors.FieldTypeMismatch;
            }
        }

        if (scope.Count != formula.FieldsConfiguration.Count)
        {
            return FormulaErrors.ArgumentsCountMismatch;
        }
        
        return scope;
    }
}
