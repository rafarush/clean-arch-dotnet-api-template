namespace CleanArchTemplate.Aplication.Abstractions.Cqrs;

public class Result<T>
{
    public T? Value { get; }
    public bool IsSuccess { get; }
    public string Error { get; }
    public ErrorType ErrorType { get; }

    protected Result(T? value, bool isSuccess, string error, ErrorType errorType)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
        ErrorType = errorType;
    }

    public static Result<T> Success(T value) => new(value, true, string.Empty, ErrorType.None);
    public static Result<T> Failure(string error, ErrorType errorType) => new(default, false, error, errorType);
}

public class Result
{
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);
    public static Result<T> Failure<T>(string error, ErrorType errorType) => Result<T>.Failure(error, errorType);
}

public enum ErrorType
{
    None = 0,
    Validation = 1,
    NotFound = 2,
    Unauthorized = 3,
    Forbidden = 4,
    Conflict = 5,
    BusinessRule = 6
}