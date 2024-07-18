
using PetFashionWebShop.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class User
    {
        
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        public string Password { get; set; }
        [Required]
        public int RoleId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }

        public virtual Role? Role { get; set; }
        public ICollection<Blog>? Blogs { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Favorite>? Favorites { get; set; }
        public ICollection<Order>? Orders { get; set; }

        public virtual UserDetail? UserDetail { get; set; }
        public ICollection<Rating>? Ratings { get; set; }
    }
}
