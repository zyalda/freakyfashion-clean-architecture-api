using FreakyFashionClient.Enums;
using FreakyFashionClient.Models;

namespace FreakyFashionClient.IServices
{
    public class LoginResult :ILoginResult
    {
        public UserManagerResponseModel UserManagerResponseModelData { get; set; }
        public ResponseStatus Status { get; set; }
    }
}
