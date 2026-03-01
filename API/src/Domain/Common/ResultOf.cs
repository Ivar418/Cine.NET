namespace API.Domain.Common;

public class ResultOf<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public T? Value { get; }
    public string? Error { get; }
    public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; }

    private ResultOf(
        bool isSuccess,
        T? value,
        string? error,
        IReadOnlyDictionary<string, string[]>? validationErrors = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        ValidationErrors = validationErrors;
    }

    public static ResultOf<T> Success(T value)
        => new ResultOf<T>(true, value, null);

    public static ResultOf<T> Failure(string error)
        => new ResultOf<T>(false, default, error);

    public static ResultOf<T> ValidationFailure(
        IReadOnlyDictionary<string, string[]> validationErrors)
        => new ResultOf<T>(false, default, "Validation failed", validationErrors);
}