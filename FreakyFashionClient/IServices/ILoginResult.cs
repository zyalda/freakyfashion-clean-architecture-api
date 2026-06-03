using FreakyFashionClient.Enums;
using FreakyFashionClient.Models;

namespace FreakyFashionClient.IServices
{
    public interface ILoginResult
    {
        public UserManagerResponseModel UserManagerResponseModelData { get; set; }
        public ResponseStatus Status { get; set; }
    }
}
