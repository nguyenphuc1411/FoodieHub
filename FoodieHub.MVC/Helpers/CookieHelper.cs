namespace FoodieHub.MVC.Helpers
{
    public static class CookieHelper
    {
        public static void SetCookie(this HttpResponse response, string key, string value, int expireDay = 30)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(expireDay), 
                HttpOnly = false,
                IsEssential = false, 
                Secure = true,
            };
            response.Cookies.Append(key, value, cookieOptions);
        }

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
                IsEssential = false,
                Secure = true,
                HttpOnly = false,
            };

            response.Cookies.Append(key, string.Empty, cookieOptions);
        }
    }
}
