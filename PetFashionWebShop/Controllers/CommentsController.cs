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
  
    public class CommentsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public CommentsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var applicationDBContext = _context.Comments.Include(c => c.Product).Include(c => c.User);
            return View(await applicationDBContext.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Product)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Create(int productId, string commentText)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Create a new Comment object with the provided productId and commentText
                var comment = new Comment
                {
                    ProductId = productId,
                    CommentText = commentText,
                    CommentDate = DateTime.Now,
                    UserId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id").Value)
                };

                if (ModelState.IsValid)
                {
                    _context.Add(comment);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index));
                    return Redirect($"/Products/Details/{productId}");
                }

                // Handle invalid model state if needed
                // You may want to provide appropriate error handling or redirection
                return RedirectToAction(nameof(Error));
            }
            else
            {
                return RedirectToAction("LoginGooglePage", "Authen");
            }

            
        }

        //public async Task<IActionResult> Create(string productName, [Bind("CommentText")] Comment comment)
        //{

        //    // Auto-populate fields
        //    comment.CommentDate = DateTime.Now;
        //    var product = _context.Products.FirstOrDefault(p => p.ProductName == productName);

        //    if (product != null)
        //    {
        //        comment.ProductId = product.ProductId;
        //    }
        //    else
        //    {
        //        // Handle case where product is not found
        //        ModelState.AddModelError("ProductName", "Invalid product name.");
        //    }
        //    comment.UserId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id").Value);

        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(comment);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    // If the model state is invalid, ensure necessary ViewData is set
        //    ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", comment.ProductId);
        //    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", comment.UserId);
        //    return View(comment);
        //}
        //public async Task<IActionResult> Create([Bind("CommentId,CommentText,CommentDate,ProductId,UserId")] Comment comment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(comment);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", comment.ProductId);
        //    ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", comment.UserId);
        //    return View(comment);
        //}

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", comment.ProductId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", comment.UserId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CommentId,CommentText,CommentDate,ProductId,UserId")] Comment comment)
        {
            if (id != comment.CommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.CommentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", comment.ProductId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", comment.UserId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Comments == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Product)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Comments == null)
            {
                return Problem("Entity set 'ApplicationDBContext.Comments'  is null.");
            }
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
          return (_context.Comments?.Any(e => e.CommentId == id)).GetValueOrDefault();
        }
    }
}
