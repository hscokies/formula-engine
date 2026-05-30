using System.Text.Json.Serialization;

namespace Domain.Expressions;

[JsonPolymorphic]
[JsonDerivedType(typeof(FieldOperand), "field")]
[JsonDerivedType(typeof(DecimalValueOperand), "decimalValue")]
[JsonDerivedType(typeof(DecimalValueOperand), "integerValue")]
[JsonDerivedType(typeof(BinaryOperation), "binaryOperation")]
[JsonDerivedType(typeof(UnaryOperator), "unaryOperator")]

public interface IOperand;

public class FieldOperand : IOperand
{
    public required string InternalName { get; set; }
}

public sealed class DecimalValueOperand : IOperand
{
    public required decimal Value { get; set; }
}

public sealed class IntegerValueOperand : IOperand
{
    public required int Value { get; set; }
} 
