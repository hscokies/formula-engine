using Application.API.Common;
using Domain.Common;
using Domain.Expressions;
using Domain.Formulas;
using Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.Caching.Memory;
using NLog;

namespace Application.API.Formulas.Submit;

public sealed class SubmitEvaluationHandler(IDataContext dataContext, IMemoryCache cache) : ICommandHandler<SubmitEvaluationCommand>
{
    private readonly Logger _logger =  LogManager.GetCurrentClassLogger();
    
    public async Task<Result> Handle(SubmitEvaluationCommand command, CancellationToken cancellationToken)
    {
        var formula = await dataContext.Formulas.FindAsync([command.FormulaId], cancellationToken);
        if (formula is null)
        {
            return FormulaErrors.NotFound;
        }

        var cacheKey = $"{CacheKey.ExpressionEvaluationResult}_{command.FormulaId}_{command.EvaluationId}";
        if (cache.TryGetValue(cacheKey, out decimal evaluationResult))
        {
            dataContext.Submissions.Add(new EvaluationSubmission
            {
                Id = command.EvaluationId,
                FormulaId = command.FormulaId,
                UserId = command.UserId,
                Result = evaluationResult
            });
            
            await dataContext.SaveChangesAsync(cancellationToken);
            
            cache.Remove(cacheKey);
            return Result.Success();
        }


        var scopeCreationResult = EvaluationScopeFactory.Create(formula, command.Arguments);
        if (!scopeCreationResult.IsSuccess)
        {
            return scopeCreationResult.Error;
        }
        
        var scope = scopeCreationResult.Value!;

        try
        {
            evaluationResult = AstEvaluator.Evaluate(formula.Expression, scope);
        }
        catch (UnknownFieldException)
        {
            return FormulaErrors.FieldNotFound;
        }
        catch (UnsupportedValueTypeException)
        {
            return FormulaErrors.UnsupportedValueType;
        }
        catch (UnsupportedOperandException ex)
        {
            _logger.Error(ex, "Encountered unsupported operand in {FormulaId} formula", command.FormulaId);
            return FormulaErrors.Malformed;
        }
        catch (UnsupportedOperatorException ex)
        {
            _logger.Error(ex, "Encountered unsupported operator in {FormulaId} formula", command.FormulaId);
            return FormulaErrors.Malformed;
        }
        
        dataContext.Submissions.Add(new EvaluationSubmission
        {
            FormulaId = command.FormulaId,
            UserId = command.UserId,
            Result = evaluationResult
        });
            
        await dataContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
