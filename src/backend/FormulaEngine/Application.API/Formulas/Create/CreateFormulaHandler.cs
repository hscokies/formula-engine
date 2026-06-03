using System.Text.Json;
using Application.API.Common;
using Domain.Common;
using Domain.Expressions;
using Domain.Formulas;
using Domain.Tokens;
using Infrastructure.Persistence.Contexts;
using NLog;

namespace Application.API.Formulas.Create;

public sealed class CreateFormulaHandler(IDataContext dataContext) : ICommandHandler<CreateFormulaCommand, CreateFormulaResult>
{
    private readonly Logger  _logger = LogManager.GetCurrentClassLogger();
    
    public async Task<Result<CreateFormulaResult>> Handle(CreateFormulaCommand command, CancellationToken cancellationToken)
    {
        var tokenizeResult = Tokenize(command.Expression);
        if (!tokenizeResult.IsSuccess)
        {
            return tokenizeResult.Error;
        }

        IOperand abstractSyntaxTree;
        var builder = new AstBuilder(tokenizeResult.Value!);
        
        try
        {
            abstractSyntaxTree = builder.Build();
        }
        catch (UnexpectedIdentifierException ex)
        {
            _logger.Error(ex, "Unable to process expression: Unexpected identifier '{Identifier}'", ex.Identifier);
            return FormulaErrors.UnexpectedIdentifier;
        }
        catch (UnexpectedTokenException ex)
        {
            _logger.Warn(ex, "Expected token to be {Expected} actual {Actual}'", ex.Expected, ex.Actual);
            return FormulaErrors.UnexpectedToken;
        }
        
        var fields = builder.Fields;
        
        var entity = new Formula
        {
            Name = command.Name,
            NameNormalized = command.Name.ToUpperInvariant(),
            OwnerId = command.UserId,
            Expression = abstractSyntaxTree,
            FieldsConfiguration = fields.Select(x => new FormulaField
            {
                InternalName = x
            }).ToArray()
        };

        dataContext.Formulas.Add(entity);
        await dataContext.SaveChangesAsync(cancellationToken);

        return new CreateFormulaResult(entity.Id, fields);
    }


    private Result<List<Token>> Tokenize(string expression)
    {
        try
        {
            return Tokenizer.Tokenize(expression);
        }
        catch (UnexpectedCharacterException ex)
        {
            _logger.Error(ex, "Unable to parse expression: Invalid character '{Character}'", ex.Character);
            return FormulaErrors.InvalidCharacter;
        }
    }
}
