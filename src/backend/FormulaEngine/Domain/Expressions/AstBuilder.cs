using System.Globalization;
using Domain.Tokens;

namespace Domain.Expressions;

public sealed class AstBuilder(List<Token> tokens)
{
    private readonly HashSet<string> _fields = [];
    public IReadOnlyCollection<string> Fields => _fields;
    
    private int _position;

    private Token Current => tokens[_position];
    private Token Previous => tokens[_position - 1];
    private bool IsEof => Current.Type is TokenType.Eof;

    
    public IOperand Build()
    {
        _fields.Clear();
        var expression = ParseExpression();
        return IsEof ? expression : throw new UnexpectedTokenException(TokenType.Eof, Current.Type);
    }

    private bool Match(params TokenType[] types)
    {
        if (!types.Any(Matches))
        {
            return false;
        }

        Advance();
        return true;
    }

    private void Consume(TokenType type)
    {
        if (!Matches(type))
        {
            throw new UnexpectedTokenException(type, Current.Type);
        }

        Advance();
    }

    private bool Matches(TokenType type)
    {
        if (IsEof)
        {
            return false;
        }

        return Current.Type == type;
    }

    private void Advance()
    {
        if (!IsEof)
        {
            _position++;
        }
    }

    private IOperand ParseExpression()
    {
        return ParseWeakOperator();
    }

    private IOperand ParseWeakOperator()
    {
        var left = ParseStrongOperator();


        while (Match(TokenType.Plus, TokenType.Minus))
        {
            var op = Previous;

            left = new BinaryOperation
            {
                Left = left,
                Operator = op.Type switch
                {
                    TokenType.Plus => BinaryOperator.Addition,
                    TokenType.Minus => BinaryOperator.Subtraction,
                    _ => throw new UnexpectedTokenException(op.Type)
                },
                Right = ParseStrongOperator()
            };
        }

        return left;
    }

    /// <summary>
    /// Parse complex operation (*, /, %)
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private IOperand ParseStrongOperator()
    {
        var left = ParseExponent();

        while (Match(
                   TokenType.Star,
                   TokenType.Slash,
                   TokenType.Modulo))
        {
            var op = Previous;

            left = new BinaryOperation
            {
                Left = left,
                Operator = op.Type switch
                {
                    TokenType.Star => BinaryOperator.Multiplication,
                    TokenType.Slash => BinaryOperator.Division,
                    TokenType.Modulo => BinaryOperator.Modulo,
                    _ => throw new UnexpectedTokenException(op.Type)
                },
                Right = ParseUnary()
            };
        }

        return left;
    }

    /// <summary>
    /// Performs Exponent parse with following priority: 2 ^ 3 ^ 2 => 2 ^ (3 ^ 2)
    /// </summary>
    private IOperand ParseExponent()
    {
        var left = ParseUnary();

        if (Match(TokenType.Caret))
        {
            var right = ParseExponent();

            left = new BinaryOperation
            {
                Left = left,
                Operator = BinaryOperator.Exponent,
                Right = right
            };
        }

        return left;
    }

    /// <summary>
    /// Tries to parse unary expression like negation & function calls
    /// </summary>
    private IOperand ParseUnary()
    {
        if (Match(TokenType.Minus))
        {
            return new UnaryExpression
            {
                Operator = UnaryOperator.Negate,
                Operand = ParseUnary()
            };
        }

        if (!Match(TokenType.Identifier))
        {
            return ParsePrimary();
        }

        var identifier = Previous.Value;

        // not function call / parenthesis
        if (!Match(TokenType.LeftParenthesis))
        {
            _fields.Add(identifier);
            return new FieldOperand
            {
                InternalName = identifier
            };
        }

        var operand = ParseExpression();

        Consume(TokenType.RightParenthesis);

        return new UnaryExpression
        {
            Operand = operand,
            Operator = identifier.ToLower() switch
            {
                "abs" => UnaryOperator.Abs,
                "sqrt" => UnaryOperator.Sqrt,
                "ceil" => UnaryOperator.Ceil,
                "floor" => UnaryOperator.Floor,
                "round" => UnaryOperator.Round,
                _ => throw new UnexpectedIdentifierException(identifier)
            }
        };
    }

    /// <summary>
    /// Parse the highest priority tokens, like numbers and parenthesis expressions
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private IOperand ParsePrimary()
    {
        if (Match(TokenType.Integer))
        {
            return new IntegerValueOperand
            {
                Value = int.Parse(
                    Previous.Value,
                    CultureInfo.InvariantCulture)
            };
        }

        if (Match(TokenType.Decimal))
        {
            return new DecimalValueOperand
            {
                Value = decimal.Parse(
                    Previous.Value,
                    CultureInfo.InvariantCulture)
            };
        }

        if (Match(TokenType.LeftParenthesis))
        {
            var expression = ParseExpression();

            Consume(TokenType.RightParenthesis);

            return expression;
        }

        throw new UnexpectedTokenException(Current.Type);
    }
}
