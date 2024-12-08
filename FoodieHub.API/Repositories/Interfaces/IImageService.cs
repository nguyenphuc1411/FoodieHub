using FoodieHub.API.Models.DTOs.UploadImage;

namespace FoodieHub.API.Repositories.Interfaces
{
    public interface IImageService
    {
        Task<bool> UploadImageByName(UploadImageDTO dto);
        Task DeleteImage(string filePath);
    }
}
