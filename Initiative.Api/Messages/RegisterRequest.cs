namespace Initiative.Api.Messages
{
    public class RegisterRequest
    {
        public required string DisplayName  { get; set; }
        public required string EmailAddress { get; set; }
        public required string Password { get; set; }
    }
}
