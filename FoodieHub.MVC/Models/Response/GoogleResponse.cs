namespace FoodieHub.MVC.Models.Response
{
    public class GoogleResponse
    {
        public bool Success { get; set; }
        public string Data { get; set; } = default!;
        public string Message { get; set; } = default!;
    }
}
