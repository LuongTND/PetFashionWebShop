using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PetFashionWebShop.Models;


namespace PetFashionWebShop.Services.ViewComponents
{
    [ViewComponent(Name = "CreateComment")]
    public class CreateCommentComponent : ViewComponent
    {
        private readonly ApplicationDBContext _context;

        public CreateCommentComponent(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int productId)
        {
            // Tạo một đối tượng Comment mới để truyền vào View
            var comment = new Comment { ProductId = productId };

            // Trả về view và truyền đối tượng comment vào
            return View(comment);
        }

    }
}
