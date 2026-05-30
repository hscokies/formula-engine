using Domain.Expressions;

namespace Domain.Formulas;

public class Formula
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    
    public required string Name { get; set; }
    public required IOperand Expression { get; init; }
    
    public ICollection<FormulaField> FieldsConfiguration { get; init; } = [];
}
