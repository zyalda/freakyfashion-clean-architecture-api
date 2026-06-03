using FreakyFashionClient.IModels;
using FreakyFashionClient.PaginationDTO;

namespace FreakyFashionClient.Models
{
    public class DashboardCategoriesViewModel: IDashboardViewModel<CategoryModel>
    {
        public CategoryModel EntityModel { get; set; } = new CategoryModel();
        public PagedResponse PagedResponse { get; set; }
        public IList<CategoryModel> EntityModelList { get; set; } = new List<CategoryModel>();
        public UserManagerResponseModel UserManagerResponseModel { get; set; } = new UserManagerResponseModel();
    }
}
