namespace ignivault.ApiClient
{
    /// <summary>
    /// ApiResponse with a data payload of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; set; }
    }

    /// <summary>
    /// ApiResponse represents a standard response structure for API calls, indicating success or failure and providing a message.
    /// </summary>
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
