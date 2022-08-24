namespace ProjectManagement.Utility.Swagger.Auth
{
    public class LoginSchema
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
    }
}
