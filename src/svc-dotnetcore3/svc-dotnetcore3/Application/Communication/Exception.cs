using StatusCodes = Microsoft.AspNetCore.Http.StatusCodes;
using System;
using System.Security.Permissions;
using System.Runtime.Serialization;

namespace Web.API.Application.Communication
{
    [Serializable]
    // Important: This attribute is NOT inherited from Exception, and MUST be specified 
    // otherwise serialization will fail with a SerializationException stating that
    // "Type X in Assembly Y is not marked as serializable."
    public class CustomException<T> : System.Exception
    {
        /// <summary>My Custom Exception</summary>
        /// <example>Not Found Exception</example>
        private readonly T MyException;

        public CustomException()
        { }

        public CustomException(string message) : base(message)
        { }

        public CustomException(string message, System.Exception innerException) : base(message, innerException)
        { }

        public CustomException(T myException)
        {
            this.MyException = myException;
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        // Constructor should be protected for unsealed classes, private for sealed classes.
        // (The Serializer invokes this constructor through reflection, so it can be private)
        private CustomException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.MyException = (T)info.GetValue("Item", typeof(T));
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("MyException", this.MyException);

            // MUST call through to the base class to let it save its own state
            base.GetObjectData(info, context);
        }

        public T GetException()
        {
            return MyException;
        }
    }
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

    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message, int code = StatusCodes.Status401Unauthorized, string status = "Unauthorized Access") : base(message, code, status)
        { }
    }

    public class ForbiddenException : BaseException
    {
        public ForbiddenException(string message, int code = StatusCodes.Status403Forbidden, string status = "Forbidden Access") : base(message, code, status)
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
