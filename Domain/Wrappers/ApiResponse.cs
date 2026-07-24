using Domain.Enums;

namespace Domain.Wrappers
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; private set; }
        public int StatusCode { get; private set; }
        public T? Data { get; private set; }
        public string? Message { get; private set; }
        public List<string>? Errors { get; private set; }

        private ApiResponse(bool success, StatusCode statusCode, T? data, string message, List<string>? errors)
        {
            IsSuccess = success;
            StatusCode = (int)statusCode;
            Data = data;
            Message = message;
            Errors = errors;
        }

        // Success response
        public static ApiResponse<T> Success(StatusCode statusCode, T? data, string message = "Request successful")
        {
            return new ApiResponse<T>(true, statusCode, data, message, null);
        }

        // Generic Failed response
        public static ApiResponse<T> Fail(StatusCode statusCode, string message)
        {
            return new ApiResponse<T>(false, statusCode, default, message, new List<string> {});
        }

        // Validation Failed response
        public static ApiResponse<T> ValidationFailed(string message, List<string> errors)
        {
            return new ApiResponse<T>(false, Enums.StatusCode.BadRequest, default, message, errors);
        }
    }
}
