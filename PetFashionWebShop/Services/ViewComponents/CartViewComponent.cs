using Microsoft.AspNetCore.Mvc;
using PetFashionWebShop.ModelServices;
using PetFashionWebShop.Services.Helpes;

namespace PetFashionWebShop.Services.ViewComponents
{
	public class CartViewComponent : ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			var cart = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

			return View("CartPanel", new CartModel
			{
				Quantity = cart.Sum(p => p.SoLuong),
				Total = (double)cart.Sum(p => p.ThanhTien)
			});
		}
	}
}
