using FoodieHub.MVC.Models;
using FoodieHub.MVC.Models.Product;
using FoodieHub.MVC.Models.Response;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IProductImageService
    {
        Task<List<GetProductImages>> GetAllImg();
        Task<APIResponse> AddImageProduct(ProductImageDTO image);

        Task<APIResponse> AddMultipleImages(List<ProductImageDTO> images);

        Task<APIResponse> DeleteImage(int id);

        Task<APIResponse> DeleteImgByProductID(int id);
    }
}
