namespace BTBaseWebAPI.Models
{
    public class ApiResult
    {
        public int code;
        public string msg;
        public object content;
    }

    public class ErrorResult
    {
        public int errorCode;
        public string error;
    }
}