using FoodieHub.MVC.Models;
using FoodieHub.MVC.Models.Product;
using FoodieHub.MVC.Models.Response;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IProductService
    {
        Task<List<GetProductDTO>> GetAll();
        Task<APIResponse<GetProductDTO>> GetById(int id);
        Task<APIResponse> AddProduct(ProductDTO productDTO);
        Task<APIResponse> UpdateProduct(ProductDTO productDTO);

        Task<APIResponse> UpdateProductWithImg(ProductWithImgDTO productDTO);
        Task<APIResponse> UpdateProductNoneImg(ProductNoneImgDTO productDTO);

        Task<APIResponse> SetProductStatusOn(ProductStatusDTO productDTO);

        Task<APIResponse> UpdateProductQuantity(ProductSubtractQuantityDTO productDTO);

        Task<APIResponse> SetProductStatusOff(ProductStatusDTO productDTO);
        Task<APIResponse> DeleteProduct(int id);

    }
}
