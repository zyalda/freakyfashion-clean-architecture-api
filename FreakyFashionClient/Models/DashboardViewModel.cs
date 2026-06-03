using FreakyFashionClient.IModels;
using FreakyFashionClient.PaginationDTO;

namespace FreakyFashionClient.Models
{
    public class DashboardViewModel : IDashboardViewModel<ProductModel>
    {
        public ProductModel EntityModel { get; set; } = new ProductModel();
        public PagedResponse PagedResponse {  get; set; }
        public IList<ProductModel> EntityModelList { get; set; } = new List<ProductModel>();
        public UserManagerResponseModel UserManagerResponseModel { get; set; } = new UserManagerResponseModel();
    }
}
