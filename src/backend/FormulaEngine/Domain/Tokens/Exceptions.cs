namespace Domain.Tokens;

public class UnexpectedCharacterException(char character) : Exception($"Unexpected character: '{character}'")
{
    public char Character => character;
}
