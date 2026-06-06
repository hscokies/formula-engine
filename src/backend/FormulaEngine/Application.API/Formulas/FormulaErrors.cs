using Domain.Common;

namespace Application.API.Formulas;

internal static class FormulaErrors
{
    public static Error NotFound => Error.Validation("Formulas.NotFound", "Specified formula cannot be found.");
    public static Error FieldNotFound => Error.Validation("FormulaFields.NotFound", "One or more of the specified fields cannot be found.");
    
    public static Error InvalidCharacter => Error.Validation("Formulas.InvalidCharacter", "Provided formula contains one or more invalid characters.");
    public static Error UnexpectedToken => Error.Validation("Formulas.UnexpectedToken", "Encountered unexpected token while processing provided formula.");
    public static Error UnexpectedIdentifier => Error.Validation("Formulas.UnexpectedIdentifier", "Encountered unexpected identifier while processing provided formula.");
    
    
    public static Error Malformed => Error.Validation("Formulas.Malformed", "Formula appears to be malformed, please contact formula owner.");
    public static Error UnsupportedValueType => Error.Validation("Formulas.UnsupportedValueType", "Unable to evaluate expression: One or more of the provided variables are not supported.");
    public static Error FieldTypeMismatch => Error.Validation("Formulas.FieldTypeMismatch", "Unable to evaluate expression: One or more of the provided variables doesn't match target field type.");
    public static Error ArgumentsCountMismatch => Error.Validation("Formulas.ArgumentsCountMismatch", "Unable to evaluate expression: Arguments count mismatch.");
    
    public static Error UnableToEvaluate => Error.Validation("Formulas.UnableToEvaluate", "Unable to evaluate expression.");
}
