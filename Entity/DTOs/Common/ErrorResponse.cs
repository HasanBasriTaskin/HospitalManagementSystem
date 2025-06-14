namespace Entity.DTOs.Common
{
    public class ErrorResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public IDictionary<string, string[]> Errors { get; set; }
    }
} 