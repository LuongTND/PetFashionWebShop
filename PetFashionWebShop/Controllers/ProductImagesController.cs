using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Data;
using PetFashionWebShop.Models;

namespace PetFashionWebShop.Controllers
{
    public class ProductImagesController : Controller
    {
        private readonly ApplicationDBContext _context;

        public ProductImagesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: ProductImages
        public async Task<IActionResult> Index()
        {
            var applicationDBContext = _context.ProductImages.Include(p => p.Product);
            return View(await applicationDBContext.ToListAsync());
        }

        // GET: ProductImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProductImages == null)
            {
                return NotFound();
            }

            var productImage = await _context.ProductImages
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ProductImageId == id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

       
        private bool ProductImageExists(int id)
        {
          return (_context.ProductImages?.Any(e => e.ProductImageId == id)).GetValueOrDefault();
        }
    }
}
