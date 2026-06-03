using System.Text.Json.Serialization;

namespace Domain.Formulas;

public class FormulaField
{
    public required string InternalName { get; set; }
    public string? Label { get; set; }
    public FieldType Type { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FieldType
{
    Integer,
    Decimal,
}
