namespace Domain.Expressions;

public class BinaryOperation : IOperand
{
    public required IOperand Left { get; set; }
    public required BinaryOperator Operator { get; set; }
    public required IOperand Right { get; set; }
}

public enum BinaryOperator
{
    Addition,
    Subtraction,
    Multiplication,
    Division,
    Modulo,
    Exponent
}
