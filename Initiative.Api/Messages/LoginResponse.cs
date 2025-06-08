namespace Initiative.Api.Messages
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public required string Jwt { get; set; }
    }
}
