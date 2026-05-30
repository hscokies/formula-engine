namespace Domain.Formulas;

public class FormulaField
{
    public Guid Id { get; set; }

    public required string InternalName { get; set; }
    public string? Label { get; set; }
    public FieldType Type { get; set; }
}

public enum FieldType
{
    Integer,
    Decimal,
}
