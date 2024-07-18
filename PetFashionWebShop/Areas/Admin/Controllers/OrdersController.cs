using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Data;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis;

namespace PetFashionWebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDBContext _context;

        public OrdersController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Index(int? userId, string email)
        {
            var applicationDBContext = _context.Orders.Include(o => o.Discount).Include(o => o.User).AsQueryable();
            if (userId.HasValue)
            {
                applicationDBContext = applicationDBContext.Where(pi => pi.UserId == userId.Value);
            }

            if (!string.IsNullOrEmpty(email))
            {
                applicationDBContext = applicationDBContext.Where(pi => pi.User.Email.Contains(email));
            }
            ViewBag.Users = new SelectList(await _context.Users.ToListAsync(), "UserId", "Email");
            return View(await applicationDBContext.ToListAsync());
        }
        //private string GetEmail(int userId)
        //{
        //    //var order = _context.Oders.FirstOrDefault(u => u.OrderId == oderId);
        //    var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
        //    return user != null ? user.Email : "Unknown";
        //}

        //public async Task<IActionResult> Index(int? productId, string productName)
        //{
        //    var applicationDBContext = _context.ProductImages.Include(p => p.Product).AsQueryable();

        //    if (productId.HasValue)
        //    {
        //        applicationDBContext = applicationDBContext.Where(pi => pi.ProductId == productId.Value);
        //    }

        //    if (!string.IsNullOrEmpty(productName))
        //    {
        //        applicationDBContext = applicationDBContext.Where(pi => pi.Product.ProductName.Contains(productName));
        //    }

        //    // Pass the list of products to the view
        //    ViewBag.Products = new SelectList(await _context.Products.ToListAsync(), "ProductId", "ProductName");

        //    return View(await applicationDBContext.ToListAsync());
        //}

        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Discount)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Admin/Orders/Create
        public IActionResult Create()
        {
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "DiscountId", "DiscountName");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email");
            return View();
        }

        // POST: Admin/Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,UserId,OrderDate,OrderReceivedDate,StatusOrder,DiscountId,Description")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "DiscountId", "DiscountName", order.DiscountId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", order.UserId);
            return View(order);
        }

        // GET: Admin/Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "DiscountId", "DiscountName", order.DiscountId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", order.UserId);
            return View(order);
        }

        // POST: Admin/Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,UserId,OrderDate,OrderReceivedDate,StatusOrder,DiscountId,Description")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["DiscountId"] = new SelectList(_context.Discounts, "DiscountId", "DiscountName", order.DiscountId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", order.UserId);
            return View(order);
        }

        // GET: Admin/Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Discount)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'ApplicationDBContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}
