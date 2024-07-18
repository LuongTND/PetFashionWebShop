using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using PetFashionWebShop.ModelServices;
using PetFashionWebShop.Services.Helpes;
using PetFashionWebShop.Services.Hubs;

namespace PetFashionWebShop.Controllers
{
	public class PaymentController : Controller
	{
		private IHubContext<PaymentHubServer> _paymentHub;

		public PaymentController(IHubContext<PaymentHubServer> paymentHub)
		{
			_paymentHub = paymentHub;
		}
        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
        [Authorize]
		public IActionResult Index()
		{
			return View((long)(Cart.Sum(p => p.ThanhTien) + 25000));
		}

		[HttpPost]
		public IActionResult ReceivePayment([FromBody] object jsonData)
		{
			dynamic data = JsonConvert.DeserializeObject<dynamic>(jsonData.ToString());
			if ((int)data.error != 0) return Json(false);
			var amount = (int)data.data[0].amount;
			var desc = (string)data.data[0].description;

			string? idValue = desc.Split(' ').FirstOrDefault(x => x.StartsWith("id"))?.Substring(2);

			if (idValue == null) return Json(true);

			var result = int.TryParse(idValue, out int id);

			if (result)
			{
				_paymentHub.Clients.Groups(id.ToString()).SendAsync("ReceiveMoney", "Payment successfully!", amount);
			}

			return Json(true);
		}
	}
}
