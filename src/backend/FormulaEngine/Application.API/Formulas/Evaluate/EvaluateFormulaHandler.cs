using Application.API.Common;
using Domain.Common;
using Domain.Expressions;
using Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.Caching.Memory;
using NLog;

namespace Application.API.Formulas.Evaluate;

public sealed class EvaluateFormulaHandler(IReadOnlyDataContext dataContext, IMemoryCache memoryCache)
    : ICommandHandler<EvaluateFormulaCommand, EvaluateFormulaResult>
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public async Task<Result<EvaluateFormulaResult>> Handle(EvaluateFormulaCommand command,
        CancellationToken cancellationToken)
    {
        var formula = await dataContext.Formulas.FindAsync([command.Id], cancellationToken);
        if (formula is null)
        {
            return FormulaErrors.NotFound;
        }

        var scopeCreationResult = EvaluationScopeFactory.Create(formula, command.Arguments);
        if (!scopeCreationResult.IsSuccess)
        {
            return scopeCreationResult.Error;
        }

        var scope = scopeCreationResult.Value!;
        decimal evaluationResult;

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
            _logger.Error(ex, "Encountered unsupported operand in {FormulaId} formula", command.Id);
            return FormulaErrors.Malformed;
        }
        catch (UnsupportedOperatorException ex)
        {
            _logger.Error(ex, "Encountered unsupported operator in {FormulaId} formula", command.Id);
            return FormulaErrors.Malformed;
        }

        var evaluationId = Guid.NewGuid();
        memoryCache.Set($"{CacheKey.ExpressionEvaluationResult}_{formula.Id}_{evaluationId}", evaluationResult,
            new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15),
                Size = 1
            });

        return new EvaluateFormulaResult(evaluationId, evaluationResult);
    }
}
