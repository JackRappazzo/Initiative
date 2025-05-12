namespace Initiative.Api.Messages
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Jwt { get; set; }
        public string RefreshToken { get; set; }
    }
}
