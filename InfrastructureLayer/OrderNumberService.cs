using ApplicationLayer.IServices;

namespace InfrastructureLayer
{
    public class OrderNumberService : IOrderNumberService
    {
        public string Generate(int orderId)
        {
            return $"{orderId}-{Guid.NewGuid()}";
        }
    }
}
