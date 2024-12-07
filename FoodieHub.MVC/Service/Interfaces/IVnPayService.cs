using FoodieHub.MVC.Models.VnPay;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IVnPayService
    {
		string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
		PaymentResponseModel PaymentExecute(IQueryCollection collections);
	}
}
