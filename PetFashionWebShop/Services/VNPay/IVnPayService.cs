using PetFashionWebShop.ModelServices;

namespace PetFashionWebShop.Services.VNPay
{
	public interface IVnPayService
	{
		string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
		VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
	}
}
