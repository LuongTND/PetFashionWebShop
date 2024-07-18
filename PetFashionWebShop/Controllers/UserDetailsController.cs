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

namespace PetFashionWebShop.Controllers
{
    //[Authorize]
    public class UserDetailsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public UserDetailsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: UserDetails
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "Id").Value);
            var applicationDBContext = _context.UserDetails.Include(u => u.User).Where(o => o.UserId == userId); ;
            return View(await applicationDBContext.ToListAsync());

        }

        // GET: UserDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UserDetails == null)
            {
                return NotFound();
            }

            var userDetail = await _context.UserDetails
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userDetail == null)
            {
                return NotFound();
            }

            return View(userDetail);
        }

        // GET: UserDetails/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email");
            return View();
        }

        // POST: UserDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,FullName,Address,Phone,ImageUser")] UserDetail userDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", userDetail.UserId);
            return View(userDetail);
        }

        #region Edit
        //private async Task<int?> GetUserDetailIdByUserId(int userId)
        //{
        //    var userDetail = await _context.UserDetails.FirstOrDefaultAsync(ud => ud.UserId == userId);
        //    return userDetail?.Id;
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EditByUserId(int userId)
        //{
        //    // Lấy Id của UserDetail dựa trên UserId
        //    var id = await GetUserDetailIdByUserId(userId);
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    // Gọi phương thức Edit với Id đã lấy được
        //    return await Edit((int)id);
        //}
        #endregion

        // GET: UserDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UserDetails == null)
            {
                return NotFound();
            }

            var userDetail = await _context.UserDetails.FindAsync(id);
            if (userDetail == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", userDetail.UserId);
            return View(userDetail);
        }

        // POST: UserDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,FullName,Address,Phone,ImageUser")] UserDetail userDetail)
        {
            if (id != userDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userDetail);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("CheckoutView", "Cart");
                  
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserDetailExists(userDetail.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", userDetail.UserId);
            return View(userDetail);
        }

       

        private bool UserDetailExists(int id)
        {
          return (_context.UserDetails?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
