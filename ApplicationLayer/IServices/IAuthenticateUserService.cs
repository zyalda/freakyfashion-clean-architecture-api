using ApplicationLayer.Dto;

namespace ApplicationLayer.IServices
{
    public interface IAuthenticateUserService
    {
        DtoCustomer LoginToAuthenticate(string userName, string passWord, ICustomerService customerService); //, IUserTypeService userTypeService);

        UserManagerResponse GenerateToken(DtoCustomer dtoCustomer);

    }
}
