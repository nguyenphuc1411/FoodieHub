using FoodieHub.API.Models.Response;

namespace FoodieHub.API.Extentions
{
    public class ImageExtentions
    {
        private readonly IWebHostEnvironment _environment;

        public ImageExtentions(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<UploadImageResult> UploadImage(IFormFile file, string folder)
        {
            if (IsValidImage(file))
            {
                var fileExtention = Path.GetExtension(file.FileName);

                string newfileName = DateTime.Now.Ticks.ToString() + fileExtention;
                string filePath = Path.Combine(_environment.WebRootPath, "Images", folder, newfileName);

                // Kiểm tra xem thư mục đã tồn tại chưa, nếu chưa thì tạo
                if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "Images", folder)))
                {
                    Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "Images", folder));
                }

                // Lưu file vào đường dẫn
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return new UploadImageResult
                {
                    Success = true,
                    Message = "Upload file success",
                    FilePath = Path.Combine(folder, newfileName)
                };
            }
            return new UploadImageResult
            {
                Success = false,
                Message = "Invalid Image"
            };
        }
        public async Task<string> SaveImageFromBytesAsync(byte[] imageBytes, string fileName)
        {
            // Define the path to save the image in the wwwroot/images folder
            string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images","QRCodes");

            // Ensure the directory exists, if not, create it
            if (!Directory.Exists(wwwRootPath))
            {
                Directory.CreateDirectory(wwwRootPath);
            }

            // Create the full path for the image
            string filePath = Path.Combine(wwwRootPath, fileName);

            // Write the byte array as an image file
            await File.WriteAllBytesAsync(filePath, imageBytes);
            string relativePath = Path.Combine("images","QRCodes", fileName);
            return relativePath;
        }



        public async Task<UploadImageResult> UploadMutipleImages(List<IFormFile> files, string folder)
        {
            foreach (var file in files)
            {
                if (!IsValidImage(file))
                    return new UploadImageResult
                    {
                        Success = false,
                        Message = "Invalid Image"
                    };
            }
            var listFilePath = new List<string>();
            foreach (var file in files)
            {
                var fileExtention = Path.GetExtension(file.FileName);

                string newfileName = DateTime.Now.Ticks.ToString() + fileExtention;
                string filePath = Path.Combine(_environment.WebRootPath, "Images", folder, newfileName);

                // Kiểm tra xem thư mục đã tồn tại chưa, nếu chưa thì tạo
                if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "Images", folder)))
                {
                    Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "Images", folder));
                }

                // Lưu file vào đường dẫn
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                listFilePath.Add(Path.Combine(folder, newfileName));
            }
            return new UploadImageResult
            {
                Success = true,
                Message = "Upload file success",
                FilePath = listFilePath
            };
        }

        public void DeleteImage(string filePath)
        {
            var fullPath = Path.Combine(_environment.WebRootPath, "Images", filePath);

            // Kiểm tra xem tệp có tồn tại không
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);  // Xóa tệp nếu tồn tại
            }
        }


        public async Task<bool> UploadImageByName(IFormFile file,string folder, string fileName)
        {
            if (!IsValidImage(file)) return false;         
            try
            {
                var fileExtention = Path.GetExtension(file.FileName);
                string filePath = Path.Combine(_environment.WebRootPath, "Images", folder, fileName);

                if (!Directory.Exists(Path.Combine(_environment.WebRootPath, "Images", folder)))
                {
                    Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "Images", folder));
                }
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return true;
            }
            catch{return false;}                
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
