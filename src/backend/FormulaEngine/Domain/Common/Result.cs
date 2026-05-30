namespace Domain.Common;

public record Result(bool IsSuccess, Error Error)
{
    public bool IsFailure => !IsSuccess;
    
    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    
    public static implicit operator Result(Error error)
    {
        return Failure(error);
    }

    public TOut Match<TOut>(
        Func<TOut> onSuccess,
        Func<Error, TOut> onFailure)
    {
        return IsSuccess ? onSuccess() : onFailure(Error);
    }
}

public record Result<TValue>(bool IsSuccess, TValue? Value, Error Error)
{
    public static Result<TValue> Success(TValue value) => new(true, value, Error.None);
    public static Result<TValue> Failure(Error error) => new(false, default, error);

    public static implicit operator Result<TValue>(TValue value)
    {
        return Success(value);
    }
    
    public static implicit operator Result<TValue>(Error error)
    {
        return Failure(error);
    }
    
    public TOut Match<TOut>(
        Func<TValue, TOut> onSuccess,
        Func<Error, TOut> onFailure)
    {
        return IsSuccess ? onSuccess(Value!) : onFailure(Error);
    }
}
