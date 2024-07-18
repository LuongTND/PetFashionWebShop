using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using PetFashionWebShop.Models;


namespace Models
{
    public class Order
    {
        


        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
       
        [Required]
        public int UserId { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
       
        public DateTime? OrderReceivedDate { get; set; }
        [Required]
        public string StatusOrder { get; set; }

        public int? DiscountId { get; set; }
        public string? Description { get; set; }
        public virtual User? User { get; set; }
        public virtual Discount? Discount { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
