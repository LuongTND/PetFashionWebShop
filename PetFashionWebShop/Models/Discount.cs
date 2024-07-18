using Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PetFashionWebShop.Models
{
    public class Discount
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DiscountId { get; set; }
        [Required]
        public int PercentDiscount  { get; set; }
        [Required]
        public string DiscountName { get; set; }
        public ICollection<Order>? Orders { get; set; }

    }
}
