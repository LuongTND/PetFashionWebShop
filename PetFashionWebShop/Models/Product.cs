using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PetFashionWebShop.Models;
using System.Text.RegularExpressions;


namespace Models
{
    public class Product
    {
        
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public int CategoryId { get; set; }
        
        [Required]
        public int UnitStock { get; set; }

        [Required]
        public string Image { get; set; }

        public decimal? UnitPriceOld { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
        
        public string Description { get; set; }
       


        public string? Status { get; set; }
        

        public virtual Category? Category { get; set; }
        public ICollection<ProductImage>? ProductImages { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Favorite>? Favorites { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
        public ICollection<Rating>? Ratings { get; set; }
    }
}
