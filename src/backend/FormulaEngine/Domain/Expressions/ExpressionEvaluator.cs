namespace Domain.Expressions;

public static class AstEvaluator
{
    public static decimal Evaluate(
        IOperand expression,
        IReadOnlyDictionary<string, object> scope)
    {
        return EvaluateInternal(expression, scope);
    }

    private static decimal EvaluateInternal(
        IOperand operand,
        IReadOnlyDictionary<string, object> scope)
    {
        return operand switch
        {
            IntegerValueOperand integer => integer.Value,
            DecimalValueOperand @decimal => @decimal.Value,
            FieldOperand field => ResolveField(field, scope),
            UnaryOperation unary => EvaluateUnary(unary, scope),
            BinaryOperation binary => EvaluateBinary(binary, scope),
            _ => throw new UnsupportedOperandException(operand.GetType())
        };
    }

    private static decimal ResolveField(
        FieldOperand field,
        IReadOnlyDictionary<string, object> scope)
    {
        if (!scope.TryGetValue(
                field.InternalName,
                out var value))
        {
            throw new UnknownFieldException(field.InternalName);
        }

        return value switch
        {
            int i => i,
            decimal d => d,
            _ => throw new UnsupportedValueTypeException(value.GetType().Name)
        };
    }

    private static decimal EvaluateBinary(
        BinaryOperation operation,
        IReadOnlyDictionary<string, object> scope)
    {
        var left = EvaluateInternal(
            operation.Left,
            scope);

        var right = EvaluateInternal(
            operation.Right,
            scope);

        return operation.Operator switch
        {
            BinaryOperator.Addition => left + right,
            BinaryOperator.Subtraction => left - right,
            BinaryOperator.Multiplication => left * right,
            BinaryOperator.Division => left / right,
            BinaryOperator.Modulo => left % right,

            BinaryOperator.Exponent
                => (decimal)Math.Pow(
                    (double)left,
                    (double)right),

            _ => throw new UnsupportedOperatorException($"Unsupported binary operator: {operation.Operator}")
        };
    }

    private static decimal EvaluateUnary(
        UnaryOperation operation,
        IReadOnlyDictionary<string, object> scope)
    {
        var value = EvaluateInternal(
            operation.Operand,
            scope);

        return operation.Operator switch
        {
            UnaryOperator.Abs => Math.Abs(value),
            UnaryOperator.Sqrt => (decimal)Math.Sqrt((double)value),
            UnaryOperator.Ceil => Math.Ceiling(value),
            UnaryOperator.Floor => Math.Floor(value),
            UnaryOperator.Round => Math.Round(value),
            UnaryOperator.Negate => -value,
            _ => throw new UnsupportedOperatorException($"Unsupported unary operator: {operation.Operator}")
        };
    }
}
