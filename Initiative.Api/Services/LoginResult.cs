namespace Initiative.Api.Services
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public LoginErrorType ErrorType { get; set; }
        public string Jwt { get; set; }
        public string RefreshToken { get; set; }
    }

    public enum LoginErrorType
    {
        None,
        EmailDoesNotExist,
        PasswordMismatch,
        Unknown
    }
}
