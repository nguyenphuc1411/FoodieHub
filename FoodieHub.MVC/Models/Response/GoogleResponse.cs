namespace FoodieHub.MVC.Models.Response
{
    public class GoogleResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; } = default!;
        public string Message { get; set; } = default!;
    }
}
