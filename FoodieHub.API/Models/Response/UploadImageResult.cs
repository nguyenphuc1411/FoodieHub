namespace FoodieHub.API.Models.Response
{
    public class UploadImageResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = default!;
        public object FilePath { get; set; } = default!;
    }
}
