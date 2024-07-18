using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PetFashionWebShop.Services.ViewComponents
{
    public class CategoryFilterViewComponent : ViewComponent
    {
        private readonly ApplicationDBContext _context;

        public CategoryFilterViewComponent(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }
    }
}
