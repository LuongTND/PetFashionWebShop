using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Data;
using Models;

namespace PetFashionWebShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public ProductsController(ApplicationDBContext context)
        {
            _context = context;
        }



        // GET: Products
        public async Task<IActionResult> Index(int? categoryId)
        {
            var categories = await _context.Categories.ToListAsync();
            ViewData["CategoryId"] = new SelectList(categories, "CategoryId", "CategoryName", categoryId);

            var products = _context.Products
                                   .Include(p => p.Category)
                                   .Include(p => p.ProductImages)
                                   .Include(p => p.Comments)
                                   .Include(p => p.Ratings)
                                   .AsQueryable();

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            return View(await products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.Comments)
                .Include(p => p.Ratings)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.ApplicationDBContext = _context;
            return View(product);
        }

        
        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.ProductId == id)).GetValueOrDefault();
        }

        private string GetUserName(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            return user != null ? user.Name : "Unknown";
        }

        //public async Task<IActionResult> Index()
        //{
        //    var applicationDBContext = _context.Products.Include(p => p.Category).Include(p => p.ProductImages).Include(p => p.Comments).Include(p => p.Ratings);
        //    return View(await applicationDBContext.ToListAsync());
        //}
    }
}
