using StatusCodes = Microsoft.AspNetCore.Http.StatusCodes;

namespace Web.API.Application.Communication
{
    public class Response<T>
    {
        /// <summary>Response Code</summary>
        /// <example>200</example>
        public int code { get; }
        /// <summary>Response Status</summary>
        /// <example>OK</example>
        public string status { get; }
        /// <summary>Response Payload</summary>
        public T payload { get; }
        /// <summary>Response Message</summary>
        /// <example>Successfully retrieved information</example>
        public string message { get; }
        /// <summary>Response Extra</summary>
        public object extra { get; }
        protected Response(T payload, string message, object extra, int code, string status)
        {
            this.code = code;
            this.status = status;
            this.payload = payload;
            this.message = message;
            this.extra = extra;
        }
    }

    public class OkResponse<T> : Response<T>
    {

        public OkResponse(T payload, string message, object extra = null, int code = StatusCodes.Status200OK, string status = "OK") : base(payload, message, extra, code, status)
        { }
    }

    public class CreatedResponse<T> : Response<T>
    {
        public CreatedResponse(T payload, string message, object extra = null, int code = StatusCodes.Status201Created, string status = "Created") : base(payload, message, extra, code, status)
        { }
    }

    public class DeletedResponse<T> : Response<T>
    {
        public DeletedResponse(T payload, string message, object extra = null, int code = StatusCodes.Status200OK, string status = "Deleted") : base(payload, message, extra, code, status)
        { }
    }

    public class UpdatedResponse<T> : Response<T>
    {
        public UpdatedResponse(T payload, string message, object extra = null, int code = StatusCodes.Status200OK, string status = "Updated") : base(payload, message, extra, code, status)
        { }
    }
}
