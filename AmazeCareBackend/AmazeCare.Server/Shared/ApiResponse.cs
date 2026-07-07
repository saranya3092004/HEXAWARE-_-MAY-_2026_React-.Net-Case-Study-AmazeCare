namespace AmazeCare.Server.Modules.Auth.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set;  }
        public static ApiResponse<T> OK(T? data, string message = "Success.")
        {
            ApiResponse<T> response = new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
            return response;
        }

        public static ApiResponse<T> Fail(string message) // “A wrapper of type ApiResponse<T> is returned, where T represents the actual data type.”
        {
            ApiResponse<T> response = new ApiResponse<T>
            {
                Success = false,
                Message = message
            };
            return response;
        }

        public static ApiResponse<T> Created(T data, string message = "Created successfully.")
        {
            ApiResponse<T> response = new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
            return response;
        }

    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }

        public static ApiResponse OK(string message = "Success.")
        {
            ApiResponse response = new ApiResponse
            {
                Success = true,
                Message = message
            };
            return response;
        }

        public static ApiResponse Fail(string message)
        {
            ApiResponse response = new ApiResponse
            {
                Success = false,
                Message = message
            };
            return response;
        }
    }
}

             
       