namespace Solutions.GeneralResponse;

public class ApiResponse<T>
{
    public T? Result { get; set; }
    public bool Success { get; set; }
    public int StatusCode { get; }
    public string? Message { get; }

    public ApiResponse(int statusCode, bool succeeded, T? result, string? message = null)
    {
        Success = succeeded;
        StatusCode = statusCode;
        Result = result;
        Message = message;
    }

    public static ApiResponse<T> ApiOkResponse(T result, string? message = null)
    {
        return new ApiResponse<T>(200, true, result);
    }

    public static ApiResponse<T> ApiNoContentResponse(string? message = null)
    {
        return new ApiResponse<T>(204, true, default, message);
    }

    public static ApiResponse<T> ApiNotFoundResponse(string message)
    {
        return new ApiResponse<T>(404, false, default, message);
    }

    public static ApiResponse<T> ApiBadRequestResponse(string message)
    {
        return new ApiResponse<T>(400, false, default, message);
    }

    public static ApiResponse<T> ApiInternalServerErrorResponse(string message)
    {
        return new ApiResponse<T>(500, false, default, message);
    }

    public static ApiResponse<T> ErrorConnectingToTheServer(string message)
    {
        return new ApiResponse<T>(0, false, default, message);
    }

}