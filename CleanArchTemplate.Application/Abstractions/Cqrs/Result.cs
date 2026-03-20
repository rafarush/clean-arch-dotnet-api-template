using Microsoft.AspNetCore.Mvc;

namespace CleanArchTemplate.Application.Abstractions.Cqrs;

public class Result<T>
{
    public T? Value { get; }
    public bool IsSuccess { get; }
    public string Error { get; }
    public string? Message { get; }
    public ErrorType ErrorType { get; }
    public int? StatusCode { get; }

    protected Result(T? value, bool isSuccess, string error, ErrorType errorType, string? message, int? statusCode = null)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
        ErrorType = errorType;
        Message = message;
        StatusCode = statusCode;
    }

    public static Result<T> Success(T value, int statusCode = 200) => new(value, true, string.Empty, ErrorType.None, string.Empty, statusCode);
    public static Result<T> Success(T value, string message, int statusCode = 200) => new(value, true, string.Empty, ErrorType.None, message, statusCode);
    public static Result<T> Failure(string error, ErrorType errorType, string? message = null, int? statusCode = null) => new(default, false, error, errorType, message ?? string.Empty, statusCode);
    
    // Custom Errors
    public static Result<T> NotFound(string error, string? message = null) => Failure(error, ErrorType.NotFound, message, 404);
    public static Result<T> Validation(string error, string? message = null) => Failure(error, ErrorType.Validation, message, 400);
    public static Result<T> Conflict(string error, string? message = null) => Failure(error, ErrorType.Conflict, message, 409);
    public static Result<T> Unauthorized(string error, string? message = null) => Failure(error, ErrorType.Unauthorized, message, 401);
    public static Result<T> Forbidden(string error, string? message = null) => Failure(error, ErrorType.Forbidden, message, 403);
    public static Result<T> BusinessRule(string error, string? message = null) => Failure(error, ErrorType.BusinessRule, message, 422);
    public static Result<T> InternalError(string error, string? message = null) => Failure(error, ErrorType.InternalError, message, 500);
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
    BusinessRule = 6,
    InternalError = 7
}