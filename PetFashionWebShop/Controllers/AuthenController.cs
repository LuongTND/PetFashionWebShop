using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Data;
using Models;
using PetFashionWebShop.ModelServices;
using PetFashionWebShop.Services.Helpes;
using PetFashionWebShop.Services.Email;

namespace PetFashionWebShop.Controllers
{
    public class AuthenController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IMailServiceSystem _mailService;
        public AuthenController(ApplicationDBContext context, IMailServiceSystem mailService)
        {
            _context = context;
            _mailService = mailService;
        }
        public IActionResult RegisterPage()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userExists = _context.Users.Any(u => u.Email == model.Email);
                if (userExists)
                {
                    ModelState.AddModelError("", "Email is already taken.");
                }
                else
                {
                    var user = new User
                    {
                        Email = model.Email,
                        Password = model.Password, 
                        Name = model.Email, 
                        RoleId = 2
                    };
                    _context.Users.Add(user);
                    _context.SaveChanges();

                    MailContent content = new MailContent
                    {
                        To = user.Email,
                        Subject = "Pet Fashion",
                        Body = "<p><strong>Xin chào ❤ Hãy trải nhiệm mua sắm ở trên PetFashion </strong></p>"
                    };

                    await _mailService.SendMail(content);

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
                    //Them thong tin khach hang
                    var userDetail = await _context.UserDetails.FirstOrDefaultAsync(ud => ud.UserId == user.UserId);

                    if (userDetail == null)
                    {
                        // Nếu UserDetail không tồn tại, chuyển hướng tới "Create" của "UserDetails"
                        return RedirectToAction("Create", "UserDetails");
                    }
                    else
                    {
                        // Nếu UserDetail tồn tại, chuyển hướng tới "Index" của "Home"
                        return RedirectToAction("Index", "Home");
                    }
                    
                }
            }
            return View(model);
        }
        public IActionResult LoginGooglePage()
        {
            return View();
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

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password");
                }
                
            }
            return View(model);
        }
        public async System.Threading.Tasks.Task LoginGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GoogleResponse")

            }); ;

        }

        #region Authentication
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            var claims = result.Principal;

            var emailClaim = result.Principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

            if (emailClaim?.Value != null)
            {

                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == emailClaim.Value);

                if (existingUser == null)
                {

                    existingUser = new User()
                    {
                        Name = claims.FindFirstValue(ClaimTypes.Name),
                        Email = claims.FindFirstValue(ClaimTypes.Email),
                        Password = "highpass",
                        RoleId = 2
                    };

                    _context.Add(existingUser);
                    await _context.SaveChangesAsync();
                    
                    

                    MailContent content = new MailContent
                    {
                        To = emailClaim.Value,
                        Subject = "Pet Fashion",
                        Body = "<p><strong>Xin chào ❤ Hãy trải nhiệm mua sắm ở trên PetFashion </strong></p>"
                    };

                    await _mailService.SendMail(content);
                    
                }
                MailContent content2 = new MailContent
                {
                    To = emailClaim.Value,
                    Subject = "Pet Fashion",
                    Body = "<p><strong>Xin chào ❤ Hãy trải nhiệm mua sắm ở trên PetFashion </strong></p>"
                };

                await _mailService.SendMail(content2);

                List<Claim> claim = new List<Claim>()
                {
                    new Claim("Id", existingUser.UserId.ToString()),
                    new Claim(ClaimTypes.Name,existingUser.Name),
                    new Claim(ClaimTypes.Role, existingUser.RoleId.ToString()),
                    new Claim(ClaimTypes.Email, existingUser.Email)
                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(1)
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);
                //thêm thông tin 

                var userDetail = await _context.UserDetails.FirstOrDefaultAsync(ud => ud.UserId == existingUser.UserId);

                if (userDetail == null)
                {
                    // Nếu UserDetail không tồn tại, chuyển hướng tới "Create" của "UserDetails"
                    return RedirectToAction("Create", "UserDetails");
                }
                else
                {
                    // Nếu UserDetail tồn tại, chuyển hướng tới "Index" của "Home"
                    return RedirectToAction("Index", "Home");
                }
            }

            return RedirectToAction("Index", "Home");
        }
        #endregion


        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
        public async Task<IActionResult> Logout()

        {
            var gioHang = Cart;
            if (gioHang != null && gioHang.Any())
            {
                RestoreStock(gioHang);
            }

            // Xóa session giỏ hàng
            HttpContext.Session.Remove(MySetting.CART_KEY);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
        private void RestoreStock(List<CartItem> gioHang)
        {
            foreach (var item in gioHang)
            {
                var product = _context.Products.SingleOrDefault(p => p.ProductId == item.MaHh);
                if (product != null)
                {
                    product.UnitStock += item.SoLuong;
                }
            }
            _context.SaveChanges();
        }

    }
}
