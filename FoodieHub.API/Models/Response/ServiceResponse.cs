namespace FoodieHub.API.Models.Response
{
    public class ServiceResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = default!;
        public object Data { get; set; } = default!;
        public int StatusCode { get; set; }
    }
}
