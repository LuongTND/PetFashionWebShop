using Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PetFashionWebShop.Models
{
    public class Blog
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BlogId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string ContentBlog { get; set; }
        [Required]
        public DateTime BlogDate { get; set; }

        public string ImageBlog { get; set; }
        

        public virtual User? User { get; set; }
    }
}
