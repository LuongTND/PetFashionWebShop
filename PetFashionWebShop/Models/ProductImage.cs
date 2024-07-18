using Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetFashionWebShop.Models
{
    public class ProductImage
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductImageId { get; set; }

        [Required]
        public string ImageProduct { get; set; }

        [Required]
        public int ProductId { get; set; }
            
        public virtual Product? Product { get; set; }
    }
}
