using System.Text.Json.Serialization;

namespace Domain.Expressions;

[JsonPolymorphic]
[JsonDerivedType(typeof(FieldOperand), "field")]
[JsonDerivedType(typeof(DecimalValueOperand), "decimalValue")]
[JsonDerivedType(typeof(IntegerValueOperand), "integerValue")]
[JsonDerivedType(typeof(BinaryOperation), "binaryOperation")]
[JsonDerivedType(typeof(UnaryOperation), "unaryOperator")]

public abstract class IOperand;

public class FieldOperand : IOperand
{
    public required string InternalName { get; set; }
}
