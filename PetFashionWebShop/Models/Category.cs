using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PetFashionWebShop.Models;


namespace Models
{
    public class Category
    {

        
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }
        [Required]
        public bool Status { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
