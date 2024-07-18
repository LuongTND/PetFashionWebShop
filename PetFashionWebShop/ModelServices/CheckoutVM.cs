using PetFashionWebShop.Models;

namespace PetFashionWebShop.ModelServices
{
    public class CheckoutVM
    {
        public bool GiongKhachHang { get; set; }
        public string? HoTen { get; set; }
        public string? DiaChi { get; set; }
        public string? DienThoai { get; set; }
        public string? GhiChu { get; set; }

        public int? DiscountId { get; set; }

        public Discount? Discount { get; set; }
	}
}
