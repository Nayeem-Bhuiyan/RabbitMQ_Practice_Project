namespace SmartApp.Shared.Common;

public sealed class Response<T>
{
    public bool isSuccess { get; private init; }
    public string message { get; private init; } = string.Empty;
    public T? data { get; private init; }

    private Response() { }

    public static Response<T> SuccessResponse(T data, string message = "Operation completed successfully.") =>
        new() { isSuccess = true, data = data, message = message };

    public static Response<T> Failure(string message) =>
        new() { isSuccess = false, message = message };
}