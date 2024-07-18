using Data;
using MailKit;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models;
using PetFashionWebShop.ModelServices;
using PetFashionWebShop.Services;
using PetFashionWebShop.Services.Email;
using PetFashionWebShop.Services.Helpes;
using PetFashionWebShop.Services.VNPay;
using System.Globalization;
using System.Security.Claims;
using PetFashionWebShop.Models;



namespace PetFashionWebShop.Controllers
{
    
    public class CartController : Controller
    {

        private readonly ApplicationDBContext _context;
		private readonly IVnPayService _vnPayService;
        private readonly IMailServiceSystem _mailService;



        public CartController(ApplicationDBContext context, IVnPayService vnPayService, IMailServiceSystem mailService)
        {
            _context = context;
            _vnPayService = vnPayService;
            _mailService = mailService;
          
        }

        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
		public IActionResult Index()
		{
			return View(Cart);
		}
        public IActionResult AddToCart(int id, int quantity = 1, int redirect = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                var gioHang = Cart;
                var item = gioHang.SingleOrDefault(p => p.MaHh == id);



                if (item == null)
                {
                    var hangHoa = _context.Products.SingleOrDefault(p => p.ProductId == id);
                    string Ten = hangHoa.ProductName;
                    if (hangHoa == null)
                    {
                        //TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
                        //return Redirect("/404");
                        ViewBag.ErrorMessage = $"Không tìm thấy hàng hóa có mã {Ten}";
                        return View("Index", Cart);
                    }
                    item = new CartItem
                    {
                        MaHh = hangHoa.ProductId,
                        TenHH = hangHoa.ProductName,
                        DonGia = hangHoa.UnitPrice,
                        TrangThai = hangHoa.Status,
                        Hinh = hangHoa.Image ?? string.Empty,
                        SoLuong = quantity
                    };
                    gioHang.Add(item);
                    ViewBag.SuccessMessage = $"Add To Cart Product : {Ten}";
                }
                else
                {
                    //var hangHoa = _context.Products.SingleOrDefault(p => p.ProductId == id);
                    //item.TrangThai += hangHoa.Status;
                    item.SoLuong += quantity;
                }

                // check tồn kho
                if (!CheckStock(id, item.SoLuong))
                {
                    ViewBag.ErrorMessage = $"Khong du hang trong kho cho san pham nay";

                    RedirectToAction("RemoveItem", "Cart");
                    return View("Index", Cart);
                }
                //Cập nhật tồn kho
                var product = _context.Products.SingleOrDefault(p => p.ProductId == id);
                if (product != null)
                {
                    product.UnitStock -= quantity;
                    _context.SaveChanges();
                }


                // cập nhật session
                HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
                if (redirect == 0)
                {
                    return RedirectToAction("Index", "Products");
                }
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("LoginGooglePage", "Authen");
            }
            

        }

        //public IActionResult AddToCart(int id, int quantity = 1, int redirect = 0)
        //{
        //    var gioHang = Cart;
        //    var item = gioHang.SingleOrDefault(p => p.MaHh == id);

        //    var hangHoa = _context.Products.SingleOrDefault(p => p.ProductId == id);
        //    if (hangHoa == null)
        //    {
        //        // Xử lý khi không tìm thấy hàng hóa
        //        ViewBag.ErrorMessage = $"Không tìm thấy hàng hóa có mã {id}";
        //        return View("Index", Cart);
        //    }

        //    if (item == null)
        //    {
        //        if (hangHoa.UnitStock < quantity)
        //        {
        //            // Xử lý khi không đủ hàng tồn kho
        //            ViewBag.ErrorMessage = $"Không đủ hàng trong kho cho sản phẩm {hangHoa.ProductName}";
        //            RedirectToAction("RemoveItem", "Cart");
        //            return View("Index", Cart);
        //        }

        //        item = new CartItem
        //        {
        //            MaHh = hangHoa.ProductId,
        //            TenHH = hangHoa.ProductName,
        //            DonGia = hangHoa.UnitPrice,
        //            TrangThai = hangHoa.Status,
        //            Hinh = hangHoa.Image ?? string.Empty,
        //            SoLuong = quantity
        //        };
        //        gioHang.Add(item);
        //    }
        //    else
        //    {
        //        // Kiểm tra nếu tổng số lượng yêu cầu lớn hơn tồn kho hiện tại
        //        if (hangHoa.UnitStock < (item.SoLuong + quantity))
        //        {
        //            // Xử lý khi không đủ hàng tồn kho
        //            ViewBag.ErrorMessage = $"Không đủ hàng trong kho cho sản phẩm {item.TenHH}";
        //            RedirectToAction("RemoveItem", "Cart");
        //            return View("Index", Cart);
        //        }

        //        item.SoLuong += quantity;
        //    }

        //    // Cập nhật tồn kho
        //    hangHoa.UnitStock -= quantity;
        //    _context.SaveChanges();

        //    // Cập nhật session giỏ hàng
        //    HttpContext.Session.Set(MySetting.CART_KEY, gioHang);

        //    if (redirect == 0)
        //    {
        //        return RedirectToAction("Index", "Products");
        //    }
        //    return RedirectToAction("Index");
        //}


        public IActionResult AddToCartFromDetail(int id, int quantity = 1, int redirect = 0)
        {

            if (User.Identity.IsAuthenticated)
            {
                var gioHang = Cart;
                var item = gioHang.SingleOrDefault(p => p.MaHh == id);



                if (item == null)
                {
                    var hangHoa = _context.Products.SingleOrDefault(p => p.ProductId == id);
                    string Ten = hangHoa.ProductName;
                    if (hangHoa == null)
                    {
                        //TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
                        //return Redirect("/404");
                        ViewBag.ErrorMessage = $"Không tìm thấy hàng hóa có mã {Ten}";
                        return View("Index", Cart);
                    }
                    item = new CartItem
                    {
                        MaHh = hangHoa.ProductId,
                        TenHH = hangHoa.ProductName,
                        DonGia = hangHoa.UnitPrice,
                        TrangThai = hangHoa.Status,
                        Hinh = hangHoa.Image ?? string.Empty,
                        SoLuong = quantity
                    };
                    gioHang.Add(item);
                }
                else
                {

                    item.SoLuong += quantity;
                }

                // check tồn kho
                if (!CheckStock(id, item.SoLuong))
                {
                    ViewBag.ErrorMessage = $"Khong du hang trong kho cho san pham nay";
                    RedirectToAction("RemoveItem", "Cart");
                    return View("Index", Cart);
                }
                //Cập nhật tồn kho
                var product = _context.Products.SingleOrDefault(p => p.ProductId == id);
                if (product != null)
                {
                    product.UnitStock -= quantity;
                    _context.SaveChanges();
                }


                // cập nhật session
                HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
                if (redirect == 0)
                {
                    return RedirectToAction("Index", "Products");
                }
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("LoginGooglePage", "Authen");
            }

        }

        private bool CheckStock(int productId, int requestedQuantity)
        {
            var hangHoa = _context.Products.SingleOrDefault(p => p.ProductId == productId); 
            if (hangHoa == null) return false;
            return requestedQuantity <= hangHoa.UnitStock ;
        }

        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                // Cập nhật tồn kho trước khi xóa mục khỏi giỏ hàng
                var product = _context.Products.SingleOrDefault(p => p.ProductId == id);
                if (product != null)
                {
                    product.UnitStock += item.SoLuong;
                    _context.SaveChanges();
                }
                gioHang.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }

		public IActionResult RemoveItem(int id)
		{
			var gioHang = Cart;
			var item = gioHang.SingleOrDefault(p => p.MaHh == id);
			if (item != null)
			{
                // Lưu số lượng sản phẩm trước khi thay đổi
                int removedQuantity = item.SoLuong > 1 ? 1 : item.SoLuong;
                if (item.SoLuong > 1)
				{
					item.SoLuong -= 1;
				}
				else
				{
					gioHang.Remove(item);
				}
				HttpContext.Session.Set(MySetting.CART_KEY, gioHang);

                // Cập nhật tồn kho
                var product = _context.Products.SingleOrDefault(p => p.ProductId == id);
                if (product != null)
                {
                    product.UnitStock += removedQuantity;
                    _context.SaveChanges();
                }
            }
			return RedirectToAction("Index");
		}


		public async Task<IActionResult> CheckoutView()
		{
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "DiscountId", "DiscountName");

			if (Cart.Count == 0)
			{
				return Redirect("/");
			}
			return View(Cart);

		}
        #region CheckOutOld
        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutVM model, string payment = "COD")
        {
            if (ModelState.IsValid)
            {
                

                int userId = int.Parse((User.Claims.FirstOrDefault(c => c.Type == "Id")).Value);
                var nameClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);




                var khachHang = new UserDetail();
                khachHang = _context.UserDetails.SingleOrDefault(kh => kh.UserId == userId);

                var hoadon = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    StatusOrder = "Đã Đặt Hàng",
                    DiscountId = model.DiscountId,
                    Description = "[ Đặt Hàng Thành Công ] [ "+ payment + " ] Total : "  + (int)Cart.Sum(p => p.ThanhTien) + " + 25k ship " + " ||Họ Tên : " + nameClaim.Value+ " ||Địa chỉ : " +  khachHang.Address + " ||Số Điện Thoại : " +  khachHang.Phone +" ||Ghi chú : "+ model.GhiChu
                };

                _context.Database.BeginTransaction();
                try
                {

                    _context.Add(hoadon);
                    _context.SaveChanges();

                    var cthds = new List<OrderDetail>();
                    foreach (var item in Cart)
                    {
                        cthds.Add(new OrderDetail
                        {
                            OrderId = hoadon.OrderId,
                            ProductId = item.MaHh,
                            UnitPrice = item.DonGia,
                            UnitStock = item.SoLuong,
                            Description = DateTime.Now.ToString()
                        });
                    }
                    _context.AddRange(cthds);
                    _context.SaveChanges();
                    _context.Database.CommitTransaction();

                    MailContent content = new MailContent
                    {
                        To = emailClaim.Value,
                        Subject = "Pet Fashion - Đơn hàng đã được đặt thành công",
                        Body = "Bạn đã Đặt hàng thành công"  };
                    
                     await _mailService.SendMail(content);


                    if (payment == "Thanh toán VNPay")
                    {
                        var vnPayModel = new VnPaymentRequestModel
                        {
                            Amount = (double)(Cart.Sum(p => p.ThanhTien) + 25000),
                            CreatedDate = DateTime.Now,
                            Description = $"{model.GhiChu}",
                            FullName = model.HoTen,
                            OrderId = new Random().Next(1000, 100000),
                            DiscountId = model.DiscountId

                        };
                        return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
                    }
                    if (payment == "Tạo mã QR Thanh Toán")
                    {
                        var checkoutQR = new CheckoutQRModel
                        {
                            Amount = (double)(Cart.Sum(p => p.ThanhTien) + 25000),
                            Description = $"{model.GhiChu}",
                            DiscountId = model.DiscountId
                        };
                        // nghiên cứu lại chổ này : truyền theo kiểu như này return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
                        // truyền sang add dữ liệu tương tự (order detail vẫn lấy từ sesion bth)  xong nhớ : set lại giỏ hàng
                        // Redirect sang trang trang có mã qr , xong tạo model chứa CheckoutQRModel tiếp 
                        // hoặc cũng có thể làm theo hướng , cho lưu vào hết data xong mới chuyển tiền , haizzz cái này phương án cuối 
                        return RedirectToAction("Index", "Payment");
                    }


                    //set lại giỏ hàng = 0
                    HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

                    return View("Success");
                }
                catch
                {
                    _context.Database.RollbackTransaction();
                }



            }

            return View(Cart);
        }
        #endregion
        #region New0PhuHop
        //[HttpPost]
        //public IActionResult CheckoutNew(CheckoutVM model, string payment = "Thanh toán VNPay")
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (payment == "COD")
        //        {
        //            int userId = int.Parse((User.Claims.FirstOrDefault(c => c.Type == "Id")).Value);
        //            var nameClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        //            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

        //            var khachHang = new UserDetail();
        //            if (model.GiongKhachHang)
        //            {
        //                khachHang = _context.UserDetails.SingleOrDefault(kh => kh.UserId == userId);
        //            }
        //            else
        //            {
        //                var userDetail = new UserDetail
        //                {
        //                    UserId = userId,
        //                    FullName = nameClaim.Value,
        //                    Address = model.DiaChi ?? khachHang.Address,
        //                    Phone = model.DienThoai ?? khachHang.Phone,
        //                    ImageUser = "https://cdn-icons-png.flaticon.com/512/15383/15383003.png"
        //                };
        //                _context.UserDetails.Add(userDetail);
        //                _context.SaveChanges();

        //            }

        //            var hoadon = new Order
        //            {
        //                UserId = userId,
        //                OrderDate = DateTime.Now,
        //                StatusOrder = "1",
        //                DiscountId = model.DiscountId,
        //                Description = model.GhiChu
        //            };

        //            _context.Database.BeginTransaction();








        //            try
        //            {

        //                _context.Add(hoadon);
        //                _context.SaveChanges();

        //                var cthds = new List<OrderDetail>();
        //                foreach (var item in Cart)
        //                {
        //                    cthds.Add(new OrderDetail
        //                    {
        //                        OrderId = hoadon.OrderId,
        //                        ProductId = item.MaHh,
        //                        UnitPrice = item.DonGia,
        //                        UnitStock = item.SoLuong

        //                    });
        //                }
        //                _context.AddRange(cthds);
        //                _context.SaveChanges();
        //                _context.Database.CommitTransaction();

        //                HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

        //                return View("Success");
        //            }
        //            catch
        //            {
        //                _context.Database.RollbackTransaction();
        //            }
        //        }
        //        var vnPayModel = new VnPaymentRequestModel
        //        {
        //            Amount = (double)Cart.Sum(p => p.ThanhTien),
        //            CreatedDate = DateTime.Now,
        //            Description = $"{model.HoTen} {model.DienThoai}",
        //            FullName = model.HoTen,
        //            OrderId = new Random().Next(1000, 100000)
        //        };
        //        return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));



        //    }

        //    return View(Cart);
        //}
        #endregion
        public IActionResult UpdateCartTotal(int discountId)
        {
            // Gọi hàm GetDiscountValue để lấy phần trăm giảm giá
            int? discountPercent = GetDiscountValue(discountId);
            double discountValue = (double)discountPercent;
            var cart = Cart;

            // Cập nhật giá trị cho từng sản phẩm trong giỏ hàng
            foreach (var item in cart)
            {
                item.DonGia *= (decimal)(1 - (discountValue / 100));
            }

            // Lưu lại giỏ hàng sau khi cập nhật
            SaveCart(cart); // Giả sử có một hàm SaveCart() để lưu lại giỏ hàng

            // Trả về giá trị mới của Cart.Sum(p => p.ThanhTien) để cập nhật trên view
            return Json(new { newTotal = Cart.Sum(p => p.ThanhTien) });
        }
        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.Set(MySetting.CART_KEY, cart);
        }   

        private int? GetDiscountValue(int discountId)
        {
            var discount = _context.Discounts.FirstOrDefault(d => d.DiscountId == discountId);
            return discount != null ? discount.PercentDiscount : (int?)null;
        }


        public IActionResult PaymentSuccess()
		{
            //set lại giỏ hàng = 0
            HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

            return View("Success");
		}
		public IActionResult PaymentSuccessQR()
		{
			//set lại giỏ hàng = 0
			HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

			return View("PaymentSuccessQR");
		}
		public IActionResult PaymentFail()
		{
			return View();
		}

        public async Task<IActionResult> PaymentCallBackAsync(VnPaymentRequestModel model)
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }
            // Lưu đơn hàng vô database
            //set lại giỏ hàng = 0
            HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

            TempData["Message"] = $"Thanh toán VNPay thành công";
            return RedirectToAction("PaymentSuccess");
        }

        #region PaymentQRDaComentCode
        [HttpPost]
        public async Task<IActionResult> CheckoutQRAsync(CheckoutQRModel model)
        {
            if (ModelState.IsValid)
            {


                int userId = int.Parse((User.Claims.FirstOrDefault(c => c.Type == "Id")).Value);
                var nameClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                

                var khachHang = new UserDetail();
                khachHang = _context.UserDetails.SingleOrDefault(kh => kh.UserId == userId);
                var hoadon = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    StatusOrder = "Đã Thanh Toán, Chờ Xác Thực",
                    DiscountId = model.DiscountId,
                    Description = "[ Thanh toán qua QRCode ] Total : " + (int)Cart.Sum(p => p.ThanhTien) + " + 25k ship " + " ||Họ Tên : " + nameClaim.Value + " ||Đại chỉ : " +  khachHang.Address + " ||Số Điện Thoại : " + khachHang.Phone + " ||Ghi chú : " + model.Description
                };

                _context.Database.BeginTransaction();
                try
                {

                    _context.Add(hoadon);
                    _context.SaveChanges();

                    var cthds = new List<OrderDetail>();
                    foreach (var item in Cart)
                    {
                        cthds.Add(new OrderDetail
                        {
                            OrderId = hoadon.OrderId,
                            ProductId = item.MaHh,
                            UnitPrice = item.DonGia,
                            UnitStock = item.SoLuong,
                            Description = DateTime.Now.ToString()

                        });
                    }
                    _context.AddRange(cthds);
                    _context.SaveChanges();
                    _context.Database.CommitTransaction();
                    MailContent content = new MailContent
                    {
                        To = emailClaim.Value,
                        Subject = "Pet Fashion - Đơn hàng đã được đặt thành công",
                        Body = ""    };
                    await _mailService.SendMail(content);
                    HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

                    return View("Success");
                }
                catch
                {
                    _context.Database.RollbackTransaction();
                }



            }

            return View(Cart);
        }
        #endregion 

        //private async Task SendOrderConfirmationEmail(string email)
        //{
        //    MailContent content = new MailContent
        //    {
        //        To = email,
        //        Subject = "Pet Fashion - Đơn hàng đã được đặt",
        //        Body = "<p><strong>Xin chào!</strong></p><p>Đơn hàng của bạn đã được đặt thành công.</p>"
        //    };

        //    await _mailService.SendMail(content);
        //}


        //[HttpPost]
        //public async Task<IActionResult> Checkout(CheckoutModel model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);
        //    bool isCheckedOut = await DoCheckout(model);
        //    if (!isCheckedOut)
        //        return RedirectToAction("Index");
        //    return RedirectToAction("Index");
        //}
        //private List<CartItem> GetCart()
        //{
        //    return HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
        //}

        //public async Task<bool> DoCheckout(CheckoutModel model)
        //{
        //    using var transaction = _context.Database.BeginTransaction();
        //    try
        //    {

        //        var userId = User.Claims.FirstOrDefault(c => c.Type == "Id");
        //        var nameClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);


        //        var cart = GetCart();

        //        var order = new Order
        //        {
        //            UserId = int.Parse(userId.Value),
        //            OrderDate = DateTime.UtcNow,
        //            Description = model.PaymentMethod,
        //            DiscountId = model.DiscountId,
        //            StatusOrder = "1"
        //        };
        //        _context.Orders.Add(order);
        //        _context.SaveChanges();

        //        var userDetail = new UserDetail
        //        {
        //            UserId = int.Parse(userId.Value),
        //            FullName = nameClaim.Value,
        //            Address = model.Address,
        //            Phone = model.MobileNumber,
        //            ImageUser = "https://cdn-icons-png.flaticon.com/512/15383/15383003.png"
        //        };
        //        _context.UserDetails.Add(userDetail);
        //        _context.SaveChanges();


        //        foreach (var item in cart)
        //        {
        //            var orderDetail = new OrderDetail
        //            {
        //                ProductId = item.MaHh,
        //                OrderId = order.OrderId,
        //                UnitStock = item.SoLuong,
        //                UnitPrice = item.DonGia
        //            };
        //            _context.OrderDetails.Add(orderDetail);

        //            // update stock here

        //            //khả năng update luôn trong giỏ hàng
        //        }

        //        _context.SaveChanges();
        //        transaction.Commit();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {

        //        return false;
        //    }
        //}
    }
}
