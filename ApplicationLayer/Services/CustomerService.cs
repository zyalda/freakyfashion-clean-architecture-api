using ApplicationLayer.Dto;
using ApplicationLayer.Interfaces;
using ApplicationLayer.IServices;
using DomainLayer.Entites;

namespace ApplicationLayer.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork mapperUnitOfWork;

        public CustomerService(IUnitOfWork unitOfWork, IMapperUnitOfWork mapperUnitOfWork)
        {
            _unitOfWork = unitOfWork;
            this.mapperUnitOfWork = mapperUnitOfWork;
        }

        public DtoCustomer AddNewCustomer(Customer customer)
        {
            _unitOfWork.CustomerRepository.Add(customer);
            _unitOfWork.Complete();

            return mapperUnitOfWork.Mapper<Customer, DtoCustomer>().MapEntity(customer);
        }

        public DtoCustomer GetCustomerToLogin(string name, string passWord)
        {
            var allusers = _unitOfWork.CustomerRepository.GetAll().ToList();

            var existedCustomer = allusers.SingleOrDefault(x => string.Equals(x.Name, name,     StringComparison.OrdinalIgnoreCase));

            if (existedCustomer == null)
            {
                
                bool isPasswordUsedByAnyone = allusers.Any(x => string.Equals(x.PassWord, passWord,     StringComparison.OrdinalIgnoreCase));
                
                if (isPasswordUsedByAnyone)
                    return null;

                var custm = new Customer { Name = name, PassWord = passWord };
                _unitOfWork.CustomerRepository.Add(custm);
                _unitOfWork.Complete();
                return mapperUnitOfWork.Mapper<Customer, DtoCustomer>().MapEntity(custm);
            }

            bool isPasswordCorrect = string.Equals(existedCustomer.PassWord, passWord, StringComparison.Ordinal);

            if (!isPasswordCorrect)
                return null;

            return mapperUnitOfWork.Mapper<Customer, DtoCustomer>().MapEntity(existedCustomer);
        }
    }
}
