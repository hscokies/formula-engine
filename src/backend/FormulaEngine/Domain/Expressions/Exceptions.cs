using Domain.Tokens;

namespace Domain.Expressions;

public sealed class UnexpectedTokenException : Exception
{
    public TokenType? Expected { get; }
    public TokenType Actual { get; }

    public UnexpectedTokenException(TokenType expected, TokenType actual) : base($"Expected: {expected}, Actual: {actual}")
    {
        Expected = expected;
        Actual = actual;
    }

    public UnexpectedTokenException(TokenType actual) : base($"Unexpected token: {actual}")
    {
        Actual = actual;
    }
}

public sealed class UnexpectedIdentifierException(string identifier) : Exception($"Unexpected identifier: '{identifier}'")
{
    public string Identifier => identifier;
}

public sealed class UnknownFieldException(string internalName) : Exception
{
    public string FieldName => internalName;
}

public sealed class UnsupportedValueTypeException(string valueType) : Exception
{
    public string ValueType => valueType;
}
