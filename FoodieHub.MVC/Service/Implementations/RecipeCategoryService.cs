using FoodieHub.MVC.Models.DTOs;
using FoodieHub.MVC.Models.Categories;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FoodieHub.MVC.Service.Implementations
{
    public class RecipeCategoryService : IRecipeCategoryService
    {
        private readonly HttpClient _httpClient;

        public RecipeCategoryService(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient("MyAPI");
        }


        // Lấy danh sách tất cả Recipe Categories
        public async Task<IEnumerable<GetRecipeCategoryDTO>?> GetAll()
        {
            var response = await _httpClient.GetAsync("RecipeCategories");
            return await response.Content.ReadFromJsonAsync<IEnumerable<GetRecipeCategoryDTO>>();          
        }
        // Thêm mới Recipe Category
        public async Task<APIResponse> AddRecipeCategory(RecipeCategoryDTO recipeCategoryDTO)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(recipeCategoryDTO.CategoryID.ToString()), "CategoryID");
            content.Add(new StringContent(recipeCategoryDTO.CategoryName), "CategoryName");

            // Xử lý file nếu có
            if (recipeCategoryDTO.ImageURL != null)
            {
                var fileContent = new StreamContent(recipeCategoryDTO.ImageURL.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(recipeCategoryDTO.ImageURL.ContentType);
                content.Add(fileContent, "ImageURL", recipeCategoryDTO.ImageURL.FileName);
            }

            // Gửi POST request tới API
            var response = await _httpClient.PostAsync("RecipeCategories", content);

            // Xử lý phản hồi và trả về APIResponse
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<APIResponse>();
                return data;
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return new APIResponse { Success = false, Message = $"Error adding recipe category: {errorMessage}" };
            }
        }


        // Xóa Recipe Category
        public async Task<APIResponse> DeleteRecipeCategory(RecipeCategoryDTO recipeCategoryDTO)
        {
            var response = await _httpClient.DeleteAsync($"api/RecipeCategories/{recipeCategoryDTO.CategoryID}");
            return await HandleResponse(response);
        }

        public async Task<APIResponse> UpdateRecipeStatus(RecipeCategoryStatusDTO recipeCategoryStatusDTO)
        {
            var jsonContent = JsonContent.Create(recipeCategoryStatusDTO);
            var response = await _httpClient.PutAsync("RecipeCategories/updatestatusrecipecategory", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<APIResponse>();
            }
            else
            {
                // Đọc nội dung lỗi nếu response không thành công
                var errorContent = await response.Content.ReadAsStringAsync();
                return new APIResponse
                {
                    Success = false,
                    Message = $"Failed to update recipe category: {errorContent}"
                };
            }
        }


        public async Task<APIResponse> UpdateRecipeCategoryNoneImg(RecipeCategoryNoneImgDTO recipeCategoryDTO)
        {
            var jsonContent = JsonContent.Create(recipeCategoryDTO);
            var response = await _httpClient.PutAsync("RecipeCategories/updaterecipecategorynoneimg", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<APIResponse>();
            }
            else
            {
                // Đọc nội dung lỗi nếu response không thành công
                var errorContent = await response.Content.ReadAsStringAsync();
                return new APIResponse
                {
                    Success = false,
                    Message = $"Failed to update recipe category: {errorContent}"
                };
            }
        }

        // Cập nhật Recipe Category kèm hình ảnh
        public async Task<APIResponse> UpdateRecipeCategoryWithImg(RecipeCategoryWithImgDTO recipeCategoryDTO)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(recipeCategoryDTO.CategoryID.ToString()), "CategoryID");
            content.Add(new StringContent(recipeCategoryDTO.CategoryName), "CategoryName");

            if (recipeCategoryDTO.ImageURL != null)
            {
                var fileContent = new StreamContent(recipeCategoryDTO.ImageURL.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(recipeCategoryDTO.ImageURL.ContentType);
                content.Add(fileContent, "ImageURL", recipeCategoryDTO.ImageURL.FileName);
            }
            var response = await _httpClient.PutAsync("RecipeCategories/updaterecipecategorytwithimg", content);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<APIResponse>();
            }
            else
            {
                // Đọc nội dung lỗi nếu response không thành công
                var errorContent = await response.Content.ReadAsStringAsync();
                return new APIResponse
                {
                    Success = false,
                    Message = $"Failed to update recipe category: {errorContent}"
                };
            }
        }

        // Phương thức xử lý phản hồi API
        private async Task<APIResponse> HandleResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<APIResponse>() ?? new APIResponse { Success = true };
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return new APIResponse
                {
                    Success = false,
                    Message = errorMessage
                };
            }
        }
    }
}
