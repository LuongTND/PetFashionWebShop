using Models;

namespace PetFashionWebShop.ModelServices
{
    public class CartItem
    {
        public int MaHh { get; set; }
        public string Hinh { get; set; }
        public string TenHH { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }

        public string TrangThai { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;

        public Product? Product { get; set; }
    }
}
