using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Data;
using PetFashionWebShop.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;

namespace PetFashionWebShop.Controllers
{
   
    public class FavoritesController : Controller
    {
        private readonly ApplicationDBContext _context;

        public FavoritesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Favorites
        public async Task<IActionResult> Index()
        {
            // Lấy UserId từ các claim của người dùng hiện tại
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id").Value);

            // Truy vấn danh sách yêu thích từ cơ sở dữ liệu dựa trên UserId
            var favorites = await _context.Favorites
                                          .Where(f => f.UserId == userId)
                                          .Include(f => f.Product) // Giả sử bạn muốn bao gồm thông tin sản phẩm
                                          .ToListAsync();

            // Truyền danh sách yêu thích vào view
            return View(favorites);
        }

        
        // GET: Favorites/Create
        //public IActionResult Create()
        //{
        //    ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName");
        //    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email");
        //    return View();
        //}

        // POST: Favorites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int productId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id").Value);

                var favorite = new Favorite
                {
                    ProductId = productId,
                    UserId = userId
                };

                if (ModelState.IsValid)
                {
                    _context.Add(favorite);
                    await _context.SaveChangesAsync();
                    ViewBag.SuccessMessage = $"Add To Product Favorite List";
                    return Redirect($"/Products/Index");
                    
                }

                // Truyền ProductId vào ViewData
                return RedirectToAction(nameof(Error));
            }
            else
            {
                return RedirectToAction("LoginGooglePage", "Authen");
            }
            
        }

        //public async Task<IActionResult> Create([Bind("FavoriteId,UserId,ProductId")] Favorite favorite)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(favorite);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", favorite.ProductId);
        //    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", favorite.UserId);
        //    return View(favorite);
        //}

        

        // GET: Favorites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Favorites == null)
            {
                return NotFound();
            }

            var favorite = await _context.Favorites
                .Include(f => f.Product)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FavoriteId == id);
            if (favorite == null)
            {
                return NotFound();
            }

            return View(favorite);
        }

        // POST: Favorites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Favorites == null)
            {
                return Problem("Entity set 'ApplicationDBContext.Favorites'  is null.");
            }
            var favorite = await _context.Favorites.FindAsync(id);
            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FavoriteExists(int id)
        {
          return (_context.Favorites?.Any(e => e.FavoriteId == id)).GetValueOrDefault();
        }
    }
}
