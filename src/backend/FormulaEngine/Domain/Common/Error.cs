namespace Domain.Common;

using System.Text.Json.Serialization;


public record Error(string Code, string Description)
{
    [JsonIgnore]
    public ErrorType Type { get; }

    protected Error(string code, string description, ErrorType type) : this(code, description)
    {
        Type = type;
    }
    
    
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);

    public static Error Failure(string code, string description)
    {
        return new Error(code, description, ErrorType.Failure);
    }

    public static Error NotFound(string code, string description)
    {
        return new Error(code, description, ErrorType.NotFound);
    }

    public static Error Problem(string code, string description)
    {
        return new Error(code, description, ErrorType.Problem);
    }

    public static Error Conflict(string code, string description)
    {
        return new Error(code, description, ErrorType.Conflict);
    }

    public static Error Validation(string code, string description)
    {
        return new Error(code, description, ErrorType.Validation);
    }
}

public record ValidationError(IDictionary<string, IEnumerable<Error>> Errors) : Error("Validation.General", "One or more validation errors occurred", ErrorType.Validation);

public enum ErrorType
{
    None = -1,
    Failure = 0,
    Validation = 1,
    Problem = 2,
    NotFound = 3,
    Conflict = 4,
}
