using Domain.Enums;

namespace Domain.Wrappers
{
    public class AppException : Exception
    {
        public StatusCode StatusCode { get; set; }
        public AppException(StatusCode statusCode, string message):base(message)
        {
            StatusCode = statusCode;
        }

        public class BadRequestException : AppException
        {
            public BadRequestException(string message) : base(StatusCode.BadRequest, message) { }
        }

        public class UnauthorizedException : AppException
        {
            public UnauthorizedException(string message) : base(StatusCode.Unauthorized, message) { }
        }

        public class ForbiddenException : AppException
        {
            public ForbiddenException(string message) : base(StatusCode.Forbidden, message) { }
        }

        public class NotFoundException : AppException
        {
            public NotFoundException(string message) : base(StatusCode.NotFound, message) { }
        }
    }
}
