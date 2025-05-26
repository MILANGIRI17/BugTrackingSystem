using System.Net;

namespace BugTracker.Shared.Helper
{
    public class ResponseHandler<T>
    {
        public bool IsSuccess { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public ResponseHandler()
        {
            
        }

        public ResponseHandler(bool isSuccess, HttpStatusCode statusCode,string message, T? data)
        {
            IsSuccess = isSuccess;
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }

        public static ResponseHandler<T> SuccessResopnse(T? data = default, string message = null, HttpStatusCode statusCode = HttpStatusCode.OK) =>
            new ResponseHandler<T>(true, statusCode, message, data);

        public static ResponseHandler<T> FailureResopnse(string message = null, HttpStatusCode statusCode =  HttpStatusCode.BadRequest, T data = default) =>
            new ResponseHandler<T>(false, statusCode, message, data);
    }
}
