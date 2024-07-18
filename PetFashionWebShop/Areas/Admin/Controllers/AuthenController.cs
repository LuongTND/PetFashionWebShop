using Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PetFashionWebShop.ModelServices;

namespace PetFashionWebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Policy = "Admin")]
    public class AuthenController : Controller
    {
        private readonly ApplicationDBContext _context;

        public AuthenController(ApplicationDBContext context)
        {
            _context = context;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.SingleOrDefault(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    List<Claim> claim = new List<Claim>()
                {
                    new Claim("Id", user.UserId.ToString()),
                    new Claim(ClaimTypes.Name,user.Name),
                    new Claim(ClaimTypes.Role, user.RoleId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
                    AuthenticationProperties properties = new AuthenticationProperties()
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddDays(1)
                    };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

                    return RedirectToAction("Index", "Home","Admin");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password");
                }

            }
            return View(model);
        }
        public IActionResult LoginGooglePage()
        {
            return View();
        }
        public async System.Threading.Tasks.Task LoginGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GoogleResponse")

            }); ;

        }

        //public async Task<IActionResult> GoogleResponse()
        //{
        //    var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        //    var claims = result.Principal;

        //    var emailClaim = result.Principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

        //    if (emailClaim?.Value != null)
        //    {

        //        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailClaim.Value);

        //        if (existingUser != null)
        //        {

        //            List<Claim> claim = new List<Claim>()
        //            {
        //                new Claim("Id", existingUser.UserId.ToString()),
        //                new Claim(ClaimTypes.Name,existingUser.Name),
        //                new Claim(ClaimTypes.Role, existingUser.RoleId.ToString())
        //            };
        //            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
        //            AuthenticationProperties properties = new AuthenticationProperties()
        //            {
        //                IsPersistent = true,
        //                ExpiresUtc = DateTime.UtcNow.AddDays(1)
        //            };
        //            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);
        //            if (existingUser.RoleId == 1)
        //            {
        //                return RedirectToAction("Index", "Home", "Admin");
        //            }
        //            else
        //            {
        //                return RedirectToAction("Notification", "Home", "Admin");
        //            }
        //        }

                

        //            return RedirectToAction("Notification", "Home", "Admin");

              

        //    }
        //    else
        //    {

        //        return RedirectToAction("Index", "Home", "Admin");
        //    }
        //}

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            var claims = result.Principal;

            var emailClaim = result.Principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

            if (emailClaim?.Value != null)
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailClaim.Value);

                if (existingUser != null)
                {
                    List<Claim> claim = new List<Claim>()
                    {
                            new Claim("Id", existingUser.UserId.ToString()),
                            new Claim(ClaimTypes.Name, existingUser.Name),
                            new Claim(ClaimTypes.Role, existingUser.RoleId.ToString())
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
                    AuthenticationProperties properties = new AuthenticationProperties()
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddDays(1)
                    };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

                    // Kiểm tra RoleId của người dùng và chuyển hướng tùy thuộc vào vai trò
                    if (existingUser.RoleId == 1)
                    {
                        return RedirectToAction("Index", "Home", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Notification", "Home", "Admin");
                    }
                }
                else
                {
                    // Người dùng không tồn tại, chuyển hướng đến trang thông báo
                    return RedirectToAction("Notification", "Home", "Admin");
                }
            }
            else
            {
                // Không có claim email, chuyển hướng đến trang chính
                return RedirectToAction("Index", "Home", "Admin");
            }
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("LoginGooglePage", "Authen");
        }
    }
}
