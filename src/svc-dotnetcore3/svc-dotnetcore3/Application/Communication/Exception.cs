using StatusCodes = Microsoft.AspNetCore.Http.StatusCodes;

namespace Web.API.Application.Communication
{
    public abstract class BaseException
    {
        /// <summary>Error Code</summary>
        /// <example>400</example>
        public int code { get; }
        /// <summary>Error Status</summary>
        /// <example>Bad Request</example>
        public string status { get; }
        /// <summary>Error Message</summary>
        /// <example>Invalid Request Parameters</example>
        public string message { get; }
        protected BaseException(string message, int code, string status)
        {
            this.code = code;
            this.status = status;
            this.message = message;
        }
    }

    public class UnauthorizedAccessException : BaseException
    {
        public UnauthorizedAccessException(string message, int code = StatusCodes.Status401Unauthorized, string status = "Unauthorized Access") : base(message, code, status)
        { }
    }

    public class NotFoundException : BaseException
    {
        public NotFoundException(string message, int code = StatusCodes.Status404NotFound, string status = "Not Found") : base(message, code, status)
        { }
    }

    public class InternalServerException : BaseException
    {
        public InternalServerException(string message, int code = StatusCodes.Status500InternalServerError, string status = "Internal Server Error") : base(message, code, status)
        { }
    }

    public class BadRequestException : BaseException
    {
        public BadRequestException(string message, int code = StatusCodes.Status400BadRequest, string status = "Bad Request") : base(message, code, status)
        { }
    }
}
