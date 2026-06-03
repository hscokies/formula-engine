using Domain.Expressions;
using Domain.Users;

namespace Domain.Formulas;

public class Formula
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    
    public Guid OwnerId { get; init; }
    public User? Owner { get; init; }
    
    public required string Name { get; set; }
    public required string NameNormalized { get; set; }
    
    public required IOperand Expression { get; init; }
    
    public ICollection<FormulaField> FieldsConfiguration { get; init; } = [];
}
