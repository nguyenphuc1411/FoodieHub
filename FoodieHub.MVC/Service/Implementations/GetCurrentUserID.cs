using FoodieHub.MVC.Models.User;
using FoodieHub.MVC.Service.Interfaces;

namespace FoodieHub.MVC.Service.Implementations
{
    public class GetCurrentUserID: IGetCurrentUserID
    {
        private readonly HttpClient _httpClient;

        public GetCurrentUserID(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        public async Task<UserDTO> GetCurrentUserId()
        {
            var response = await _httpClient.GetAsync("auth/getcurrentuser");

            if (response.IsSuccessStatusCode)
            {
                // Đọc dữ liệu JSON và trả về đối tượng UserDTO
                var content = await response.Content.ReadFromJsonAsync<UserDTO>();

                return content; // Trả về kết quả nếu API thành công
            }
            else
            {
                // Xử lý trường hợp API không thành công (có thể throw exception hoặc trả về null)
                // Nên thêm thông báo lỗi hoặc log nếu cần
                return null; // Trả về null nếu API không thành công
            }
        }
    }
}
