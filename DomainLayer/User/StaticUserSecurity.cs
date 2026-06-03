namespace DomainLayer.StaticUser
{
    public static class StaticUserSecurity
    {
        public const string Admin = "Admin";
        public const string Customer = "Customer";
        public static string Authenticate(string token)
        {
            // In a real app, you would fetch the user from a database here
            // and use a library like BCrypt to verify the password.
            return token;
        }
    }
}
