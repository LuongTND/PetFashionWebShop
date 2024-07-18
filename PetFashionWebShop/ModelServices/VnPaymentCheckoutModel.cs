namespace PetFashionWebShop.ModelServices
{
    public class VnPaymentCheckoutModel
    {
        public string? Description { get; set; }
        public double Amount { get; set; }
        public int? DiscountId { get; set; }
    }
}
