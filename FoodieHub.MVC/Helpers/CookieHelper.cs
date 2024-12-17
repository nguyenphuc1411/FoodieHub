namespace FoodieHub.MVC.Helpers
{
    public static class CookieHelper
    {
        // Đặt cookie
        public static void SetCookie(this HttpResponse response, string key, string value, int expireDay = 30)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(expireDay),
                HttpOnly = false, // Không bắt buộc, có thể đặt lại nếu cần bảo mật XSS
                IsEssential = false, // Giữ nguyên nếu không cần thiết cookie
                Secure = false, // Đặt thành false để cookie hoạt động trên HTTP
            };
            response.Cookies.Append(key, value, cookieOptions);
        }

        // Lấy cookie
        public static string? GetCookie(this HttpRequest request, string key)
        {
            return request.Cookies[key];
        }

        // Xóa cookie
        public static void DeleteCookie(this HttpResponse response, string key)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1),
                IsEssential = false, // Giữ nguyên nếu không cần thiết cookie
                Secure = false, // Đặt thành false để cookie có thể bị xóa trên HTTP
                HttpOnly = false, // Có thể giữ nguyên hoặc bật nếu cần bảo mật hơn
            };

            response.Cookies.Append(key, string.Empty, cookieOptions);
        }
    }

}
