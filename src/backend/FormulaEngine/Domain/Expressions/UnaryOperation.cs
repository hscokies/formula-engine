using System.Text.Json.Serialization;

namespace Domain.Expressions;

public sealed class UnaryOperation : IOperand
{
    public IOperand Operand { get; set; }
    public UnaryOperator Operator { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UnaryOperator
{
    Abs,
    Sqrt,
    Ceil,
    Floor,
    Round,
    Negate
}
