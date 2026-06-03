using Domain.Users;

namespace Domain.Formulas;

public sealed class EvaluationSubmission
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public decimal Result { get; init; }
    
    public Guid UserId { get; init; }
    public User? User { get; init; }
    
    public Guid FormulaId { get; init; }
    public Formula? Formula { get; init; }
}
