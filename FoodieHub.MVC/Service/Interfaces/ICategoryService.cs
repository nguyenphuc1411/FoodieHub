
using FoodieHub.MVC.Models;
using FoodieHub.MVC.Models.Response;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAll();

        Task<CategoryDTO> GetProductCategoryById(int id);

        Task<APIResponse> AddNewProductCategory(CategoryDTO categoryDTO);

        

        Task<APIResponse> DeleteProductCategory(int id);

        Task<APIResponse> UpdateProductCategory(CategoryDTO categoryDTO);
    }
}
