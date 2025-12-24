using Microsoft.AspNetCore.Mvc;

namespace CleanArchTemplate.Aplication.Abstractions.Cqrs;

public class Result<T>
{
    public T? Value { get; }
    public bool IsSuccess { get; }
    public string Error { get; }
    public string? Message { get; }
    public ErrorType ErrorType { get; }

    protected Result(T? value, bool isSuccess, string error, ErrorType errorType, string? message)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
        ErrorType = errorType;
        Message = message;
    }

    public static Result<T> Success(T value) => new(value, true, string.Empty, ErrorType.None, string.Empty);
    public static Result<T> Success(T value, string message) => new(value, true, string.Empty, ErrorType.None, message);
    public static Result<T> Failure(string error, ErrorType errorType) => new(default, false, error, errorType, string.Empty);
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