using FoodieHub.API.Models.DTOs.UploadImage;
using FoodieHub.API.Repositories.Interfaces;
using System.Xml;

namespace FoodieHub.API.Repositories.Implementations
{
    public class ImageService : IImageService
    {
        public async Task DeleteImage(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                await Task.CompletedTask;
            }
            else
            {
                throw new FileNotFoundException("File not found.", filePath);
            }
        }

        public async Task<bool> UploadImageByName(UploadImageDTO dto)
        {
            if (!IsValidImage(dto.File)) return false; 
            try
            {
                // Tạo thư mục nếu chưa tồn tại
                var directory = Path.GetDirectoryName(dto.FilePath);
                if (!Directory.Exists(directory) && directory!=null)
                {
                    Directory.CreateDirectory(directory);
                }

                // Sử dụng FileStream để lưu tệp vào đường dẫn được chỉ định
                using (var stream = new FileStream(dto.FilePath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


        public static bool IsValidImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return false;
            var allowExtentions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            var fileExtention = Path.GetExtension(file.FileName);
            if (allowExtentions.Contains(fileExtention))
            {
                return true;
            }
            return false;
        }
    }
}
