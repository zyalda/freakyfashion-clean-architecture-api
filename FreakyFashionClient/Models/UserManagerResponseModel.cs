namespace FreakyFashionClient.Models
{
    public class UserManagerResponseModel
    {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string PassWord { get; set; } = string.Empty;
            public string AccessToken { get; set; }
            public string TokenType { get; set; }
            public string ExpiresIn { get; set; }
    }
}
