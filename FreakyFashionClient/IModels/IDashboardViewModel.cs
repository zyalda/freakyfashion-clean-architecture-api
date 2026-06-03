using FreakyFashionClient.Models;
using FreakyFashionClient.PaginationDTO;

namespace FreakyFashionClient.IModels
{
    public interface IDashboardViewModel<T> where T : class
    {
        T EntityModel { get; set; }
        PagedResponse PagedResponse{ get; set; }
        IList<T> EntityModelList { get; set; }
        UserManagerResponseModel UserManagerResponseModel { get; set; }
    }
}