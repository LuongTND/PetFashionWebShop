
using Microsoft.EntityFrameworkCore;
using Models;
using PetFashionWebShop.Models;

namespace Data
{
    public partial class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext() { }
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(new List<Role>()
            {
                new Role {RoleId = 1, RoleName = "Admin"},
                new Role {RoleId = 2, RoleName = "Customer"},
                new Role {RoleId = 3, RoleName = "Guest"}
            });
            modelBuilder.Entity<User>().HasData(new List<User>()
            {
                new User {UserId = 1, Password="111102",RoleId=1,Email="luongfelix14@gmail.com", Name="Đức Lương Admin"},
            });

            #region CaiDatCu
            //// Role
            //modelBuilder.Entity<Role>() 
            //    .HasKey(r => r.RoleId);

            //// User
            //modelBuilder.Entity<User>()
            //    .HasKey(u => u.UserId);

            //modelBuilder.Entity<User>()
            //    .HasOne(u => u.Role)
            //    .WithMany()
            //    .HasForeignKey(u => u.RoleId);


            //// UserDetail
            //modelBuilder.Entity<UserDetail>()
            //    .HasKey(ud => ud.Id);

            //modelBuilder.Entity<UserDetail>()
            //   .HasOne(ud => ud.User)

            //   .WithMany()
            //   .HasForeignKey(ud => ud.UserId);


            //// Product
            //modelBuilder.Entity<Product>()
            //    .HasKey(p => p.ProductId);

            //modelBuilder.Entity<Product>()
            //    .HasOne(p => p.Category)
            //    //.WithMany(c => c.Products)
            //    .WithMany()
            //    .HasForeignKey(p => p.CategoryId);


            ////Product Image
            //modelBuilder.Entity<ProductImage>()
            //    .HasKey(pi => pi.ProductImageId);

            //modelBuilder.Entity<ProductImage>()
            //    .HasOne(pi => pi.Product)
            //    //.WithMany(c => c.Products)
            //    .WithMany()
            //    .HasForeignKey(pi => pi.ProductId);

            ////Comment
            //modelBuilder.Entity<Comment>()
            //    .HasKey(cm => cm.CommentId);

            //modelBuilder.Entity<Comment>()
            //    .HasOne(cm => cm.User)

            //    .WithMany()
            //    .HasForeignKey(cm => cm.UserId);

            //modelBuilder.Entity<Comment>()
            //   .HasOne(o => o.Product)

            //   .WithMany()
            //   .HasForeignKey(o => o.ProductId);

            ////Rating
            //modelBuilder.Entity<Rating>()
            //    .HasKey(rt => rt.RatingId);

            //modelBuilder.Entity<Rating>()
            //    .HasOne(rt => rt.User)

            //    .WithMany()
            //    .HasForeignKey(rt => rt.UserId);

            //modelBuilder.Entity<Rating>()
            //   .HasOne(rt =>rt.Product)

            //   .WithMany()
            //   .HasForeignKey(rt => rt.ProductId);


            //// Category
            //modelBuilder.Entity<Category>()
            //    .HasKey(c => c.CategoryId);

            //// Order
            //modelBuilder.Entity<Order>()
            //    .HasKey(o => o.OrderId);

            //modelBuilder.Entity<Order>()
            //    .HasOne(o => o.User)

            //    .WithMany()
            //    .HasForeignKey(o => o.UserId);

            //modelBuilder.Entity<Order>()
            //   .HasOne(o => o.Discount)

            //   .WithMany()
            //   .HasForeignKey(o => o.DiscountId);


            //// OrderDetail
            //modelBuilder.Entity<OrderDetail>()
            //    .HasKey(od => od.OrderDetailId);

            //modelBuilder.Entity<OrderDetail>()
            //    .HasOne(od => od.Order)

            //    .WithMany()
            //    .HasForeignKey(od => od.OrderId);


            //modelBuilder.Entity<OrderDetail>()
            //    .HasOne(od => od.Product)

            //    .WithMany()
            //    .HasForeignKey(od => od.ProductId)
            //    .OnDelete(DeleteBehavior.NoAction);


            //// Favorite 
            //modelBuilder.Entity<Favorite>()
            //    .HasKey(od => od.FavoriteId);

            //modelBuilder.Entity<Favorite>()
            //    .HasOne(od => od.User)
            //    .WithMany()
            //    .HasForeignKey(od => od.UserId);


            //modelBuilder.Entity<Favorite>()
            //    .HasOne(od => od.Product)
            //    .WithMany()
            //    .HasForeignKey(od => od.ProductId)
            //    .OnDelete(DeleteBehavior.NoAction);


            ////Blog
            //modelBuilder.Entity<Blog>()
            //    .HasKey(bl => bl.BlogId);

            //modelBuilder.Entity<Blog>()
            //   .HasOne(bl => bl.User)

            //   .WithMany()
            //   .HasForeignKey(ud => ud.UserId);


            ////Discount

            //modelBuilder.Entity<Discount>()
            //    .HasKey(dc => dc.DiscountId);
            #endregion 


            // Cấu hình cho bảng Role
            modelBuilder.Entity<Role>()
                .HasKey(r => r.RoleId);

            // Cấu hình cho bảng User
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId);

            // Cấu hình cho bảng UserDetail
            modelBuilder.Entity<UserDetail>()
                .HasKey(ud => ud.Id);

            modelBuilder.Entity<UserDetail>()
                .HasOne(ud => ud.User)
                .WithOne(u => u.UserDetail)
                .HasForeignKey<UserDetail>(ud => ud.UserId);

            // Cấu hình cho bảng Product
            modelBuilder.Entity<Product>()
                .HasKey(p => p.ProductId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            // Cấu hình cho bảng ProductImage
            modelBuilder.Entity<ProductImage>()
                .HasKey(pi => pi.ProductImageId);

            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductId);

            // Cấu hình cho bảng Comment
            modelBuilder.Entity<Comment>()
                .HasKey(cm => cm.CommentId);

            modelBuilder.Entity<Comment>()
                .HasOne(cm => cm.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(cm => cm.UserId);

            modelBuilder.Entity<Comment>()
                .HasOne(cm => cm.Product)
                .WithMany(p => p.Comments)
                .HasForeignKey(cm => cm.ProductId);

            // Cấu hình cho bảng Rating
            modelBuilder.Entity<Rating>()
                .HasKey(rt => rt.RatingId);

            modelBuilder.Entity<Rating>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(rt => rt.UserId);

            modelBuilder.Entity<Rating>()
                .HasOne(rt => rt.Product)
                .WithMany(p => p.Ratings)
                .HasForeignKey(rt => rt.ProductId);

            // Cấu hình cho bảng Category
            modelBuilder.Entity<Category>()
                .HasKey(c => c.CategoryId);

            // Cấu hình cho bảng Order
            modelBuilder.Entity<Order>()
                .HasKey(o => o.OrderId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Discount)
                .WithMany(d => d.Orders)
                .HasForeignKey(o => o.DiscountId);

            // Cấu hình cho bảng OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => od.OrderDetailId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId);

            // Cấu hình cho bảng Favorite
            modelBuilder.Entity<Favorite>()
                .HasKey(f => f.FavoriteId);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Product)
                .WithMany(p => p.Favorites)
                .HasForeignKey(f => f.ProductId);

            // Cấu hình cho bảng Blog
            modelBuilder.Entity<Blog>()
                .HasKey(b => b.BlogId);

            modelBuilder.Entity<Blog>()
                .HasOne(b => b.User)
                .WithMany(u => u.Blogs)
                .HasForeignKey(b => b.UserId);

            // Cấu hình cho bảng Discount
            modelBuilder.Entity<Discount>()
                .HasKey(d => d.DiscountId);


        }
    }
}
