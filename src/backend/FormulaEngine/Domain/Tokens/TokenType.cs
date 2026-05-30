namespace Domain.Tokens;

public enum TokenType
{
    Identifier,
    Integer,
    Decimal,

    Plus,
    Minus,
    Star,
    Slash,
    Modulo,
    Caret,

    LeftParenthesis,
    RightParenthesis,

    Eof
}
