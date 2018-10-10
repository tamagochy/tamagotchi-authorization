

namespace tamagotchi_authorization.Helpers
{
    public class ApiResult<T> where T : class
    {
        public ApiResult()
        { }

        public ApiResult(T data)
        {
            Data = data;
        }

        public ApiResult(Error error)
        {
            Error = error;
        }

        public T Data { get; set; }
        public Error Error { get; set; }
    }

    public class Error
    {
        public string Message { get; set; }
        public int Code { get; set; }
    }

    public class Error<T> : Error where T : class
    {
        public T AdditionalData { get; set; }
    }
}
