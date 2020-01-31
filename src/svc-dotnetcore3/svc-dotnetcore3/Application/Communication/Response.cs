namespace Web.API.Application.Communication
{
    public class Response
    {
        public int code { get; }
        public string status { get; }
        public object payload { get; }
        public string message { get; }
        public object extra { get; }
        public Response(object payload=null, int code=200, string status="OK", string message="Successful", object extra=null)
        {
            this.code = code;
            this.status = status;
            this.payload = payload;
            this.message = message;
            this.extra = extra;
        }
    }
}
