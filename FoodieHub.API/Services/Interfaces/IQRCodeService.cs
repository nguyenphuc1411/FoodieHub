namespace FoodieHub.API.Services.Interfaces
{
    public interface IQRCodeService
    {
        byte[] GenerateQRCode(string content);
    }
}
