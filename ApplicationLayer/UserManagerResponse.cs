namespace ApplicationLayer
{
    public class UserManagerResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PassWord { get; set; } = string.Empty;
        public string AccessToken { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public string ExpiresIn { get; set; }
    }
}
