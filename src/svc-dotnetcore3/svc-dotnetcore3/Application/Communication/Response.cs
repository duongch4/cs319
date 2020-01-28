namespace Web.API.Application.Communication
{
    public class Response
    {
        public int code { get; }
        public string status { get; }
        public object payload { get; }
        public string message { get; }
        public object extra { get; }
        public Response(int code=200, string status="OK", object payload=null, string message="Successful", object extra=null)
        {
            this.code = code;
            this.status = status;
            this.payload = payload;
            this.message = message;
            this.extra = extra;
        }
    }
}
