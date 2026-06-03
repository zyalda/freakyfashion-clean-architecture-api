using ApplicationLayer.Dto;
using ApplicationLayer.Interfaces;
using DomainLayer.Entites;

namespace InfrastructureLayer.Mapping
{
    public class AutoMapperCustomer : IMapper<Customer, DtoCustomer>
    {
        public DtoCustomer MapEntity(Customer source)
        {
            return new DtoCustomer
            {
                Id = source.Id,
                Name = source.Name,
                PassWord = source.PassWord
            };
        }
    }
}
