using FoodieHub.API.Data.Entities;
using FoodieHub.API.Models.DTOs.Product;
using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetProducts();
        Task<ServiceResponse> GetProductByID(int id);

        Task<ServiceResponse> GetProductByName(string name);
            
        Task<ServiceResponse> AddProduct(ProductDTO product);

        Task<ServiceResponse> UpdateProduct(ProductDTO product);

        Task<ServiceResponse> UpdateStockQuantity(ProductSubtractQuantityDTO product);

        Task<ServiceResponse> UpdateProductWithImg(ProductWithImgDTO product);

        Task<ServiceResponse> UpdateProductNoneImg(ProductNoneImgDTO product);

        Task<ServiceResponse> SetProductStatus(ProductStatusDTO productDto);

        Task<ServiceResponse> DeleteProduct(int id);
    }
}
