using FoodieHub.API.Repositories.Interfaces;
using QRCoder;

namespace FoodieHub.API.Repositories.Implementations
{
    public class QRCodeService: IQRCodeService
    {
        public byte[] GenerateQRCode(string content)
        {
            byte[] QRCode = new byte[0];
            if (!string.IsNullOrEmpty(content))
            {
                QRCodeGenerator qrCodeGenerator = new QRCodeGenerator();
                QRCodeData data = qrCodeGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
                BitmapByteQRCode bitmap = new BitmapByteQRCode(data);
                QRCode = bitmap.GetGraphic(20);
            }
            return QRCode;
        }
    }
}
