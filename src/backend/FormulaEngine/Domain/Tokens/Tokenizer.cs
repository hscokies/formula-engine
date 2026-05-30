using Domain.Expressions;

namespace Domain.Tokens;

public static class Tokenizer
{
    private const char DecimalSeparator = '.';
    
    public static List<Token> Tokenize(string input)
    {
        var tokens = new List<Token>();
        var i = 0;

        while (i < input.Length)
        {
            var c = input[i];

            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            if (char.IsLetter(c))
            {
                i = ParseIdentifier(input, i, tokens);
                continue;
            }

            if (char.IsDigit(c))
            {
                i = ParseNumber(input, i, tokens);
                continue;
            }

            tokens.Add(c switch
            {
                '+' => new Token(TokenType.Plus, "+"),
                '-' => new Token(TokenType.Minus, "-"),
                '*' => new Token(TokenType.Star, "*"),
                '/' => new Token(TokenType.Slash, "/"),
                '%' => new Token(TokenType.Modulo, "%"),
                '(' => new Token(TokenType.LeftParenthesis, "("),
                ')' => new Token (TokenType.RightParenthesis, ")"),
                '^' => new Token(TokenType.Caret, "^"),
                _ => throw new UnexpectedCharacterException(c)
            });

            i++;
        }

        tokens.Add(new Token(TokenType.Eof, string.Empty));

        return tokens;
    }

    private static int ParseNumber(string input, int i, List<Token> tokens)
    {
        var start = i;
        var decimalFound = false;

        while (
            i < input.Length &&
            (char.IsDigit(input[i]) ||
             input[i] is DecimalSeparator))
        {
            if (input[i] == '.')
                decimalFound = true;

            i++;
        }

        tokens.Add(
            new Token(
                decimalFound
                    ? TokenType.Decimal
                    : TokenType.Integer,
                input[start..i]));
        return i;
    }

    private static int ParseIdentifier(string input, int i, List<Token> tokens)
    {
        var start = i;
        while (i < input.Length && char.IsLetterOrDigit(input[i]))
        {
            i++;
        }

        tokens.Add(new Token(
            TokenType.Identifier,
            input[start..i]));

        return i;
    }
    
}
