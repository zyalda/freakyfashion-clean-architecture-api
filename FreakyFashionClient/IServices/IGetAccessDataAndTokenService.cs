namespace FreakyFashionClient.IServices
{
    public interface IGetAccessDataAndTokenService
    {
        public Task<LoginResult> GetTokenAccesstAsync(string userName, string passWord);
    }
}
