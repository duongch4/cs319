namespace Web.API.Application.Communication
{
    public class CustomException
    {
        public int code { get; }
        public string status { get; }
        public string message { get; }
        public CustomException(int code, string status, string message)
        {
            this.code = code;
            this.status = status;
            this.message = message;
        }
    }
}
