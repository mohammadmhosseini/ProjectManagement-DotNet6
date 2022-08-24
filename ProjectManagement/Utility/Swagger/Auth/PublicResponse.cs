namespace ProjectManagement.Utility.Swagger.Auth
{
    public class PublicResponse
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
    }
}
