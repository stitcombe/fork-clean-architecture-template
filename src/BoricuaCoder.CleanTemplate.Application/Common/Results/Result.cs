namespace BoricuaCoder.CleanTemplate.Application.Common.Results;

public enum ResultType
{
    Success,
    ValidationError,
    NotFound,
    Conflict
}

public class Result<T>
{
    public T? Value { get; }
    public ResultType Type { get; }
    public string? Error { get; }
    public IDictionary<string, string[]>? ValidationErrors { get; }

    private Result(T? value, ResultType type, string? error, IDictionary<string, string[]>? validationErrors)
    {
        Value = value;
        Type = type;
        Error = error;
        ValidationErrors = validationErrors;
    }

    public bool IsSuccess => Type == ResultType.Success;

    public static Result<T> Success(T value) =>
        new(value, ResultType.Success, null, null);

    public static Result<T> NotFound(string error) =>
        new(default, ResultType.NotFound, error, null);

    public static Result<T> Conflict(string error) =>
        new(default, ResultType.Conflict, error, null);

    public static Result<T> ValidationError(IDictionary<string, string[]> errors) =>
        new(default, ResultType.ValidationError, "Validation failed.", errors);
}
