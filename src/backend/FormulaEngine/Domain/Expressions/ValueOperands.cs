using System.Text.Json.Serialization;
using Domain.Formulas;

namespace Domain.Expressions;

[JsonPolymorphic]
[JsonDerivedType(typeof(DecimalValueOperand), nameof(FieldType.Decimal))]
[JsonDerivedType(typeof(IntegerValueOperand), nameof(FieldType.Integer))]
public interface IValue
{
    public bool TryGet<T>(out T? value);
}

public sealed class DecimalValueOperand : IOperand, IValue
{
    public required decimal Value { get; set; }
    public bool TryGet<T>(out T? value)
    {
        value = default;
        if (Value is not T decimalValue)
        {
            return false;
        }

        value = decimalValue;
        return true;
    }
}

public sealed class IntegerValueOperand : IOperand, IValue
{
    public required int Value { get; set; }
    public bool TryGet<T>(out T? value)
    {
        value = default;
        if (Value is not T integerValue)
        {
            return false;
        }

        value = integerValue;
        return true;
    }
}
