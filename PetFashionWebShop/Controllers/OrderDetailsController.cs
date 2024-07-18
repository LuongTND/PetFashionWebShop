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
    public class OrderDetailsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public OrderDetailsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: OrderDetails
        public async Task<IActionResult> Index()
        {
            //var applicationDBContext = _context.OrderDetails.Include(o => o.Order).Include(o => o.Product);
            // Lấy UserId từ các claim của người dùng hiện tại
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id").Value);

            // Lọc các order detail theo userId của người dùng hiện tại
            var applicationDBContext = _context.OrderDetails
                                                .Include(od => od.Order)
                                                .Include(od => od.Product)
                                                .Where(od => od.Order.UserId == userId);

            return View(await applicationDBContext.ToListAsync());
        }

        // GET: OrderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrderDetails == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(o => o.Order)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderDetailId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        
        private bool OrderDetailExists(int id)
        {
          return (_context.OrderDetails?.Any(e => e.OrderDetailId == id)).GetValueOrDefault();
        }
    }
}
