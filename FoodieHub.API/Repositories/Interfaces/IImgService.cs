using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Product;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface IImgService
    {
        Task<List<ProductImage>> GetAllImg();
        Task<ServiceResponse> DeleteImgByProductID(int id);
        Task<ServiceResponse> AddImage(ProductImageDTO img);
        Task<ServiceResponse> AddMultipleImages(List<ProductImageDTO> imgs);

        Task<ServiceResponse> DeleteImage(int id);
    }
}
