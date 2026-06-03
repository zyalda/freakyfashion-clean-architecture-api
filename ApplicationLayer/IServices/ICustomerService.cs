using ApplicationLayer.Dto;
using DomainLayer.Entites;

namespace ApplicationLayer.IServices
{
    public interface ICustomerService
    {
        DtoCustomer GetCustomerToLogin(string name, string passWord);
        DtoCustomer AddNewCustomer(Customer customer);
    }
}