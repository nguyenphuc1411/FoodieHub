namespace FoodieHub.API.Repositories.Interfaces
{
    public interface IQRCodeService
    {
        byte[] GenerateQRCode(string content);
    }
}
