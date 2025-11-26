namespace MobilivaCase.Data
  
{
    public enum ApiStatus
    {
        Success,
        Failed
    }

    public class ApiResponse<T>
    {
        public ApiStatus Status { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public T Data { get; set; }
    }

}
