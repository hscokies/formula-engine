namespace Domain.Expressions;

public sealed class UnaryExpression : IOperand
{
    public IOperand Operand { get; set; }
    public UnaryOperator Operator { get; set; }
}

public enum UnaryOperator
{
    Abs,
    Sqrt,
    Ceil,
    Floor,
    Round,
    Negate
}
