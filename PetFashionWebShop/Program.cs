using Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using PetFashionWebShop.ModelServices;
using PetFashionWebShop.Services.Email;
using PetFashionWebShop.Services.Hubs;
using PetFashionWebShop.Services.VNPay;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region Authentication
//Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

})
.AddCookie(options =>
{
    options.LoginPath = "/Authen/LoginGoogle";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.SlidingExpiration = true;
    options.AccessDeniedPath = "/Index";
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
    options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
	options.CallbackPath = "/signin-google";
});
#endregion

# region Authentization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireAssertion(context =>
    {
        var role = context.User.FindFirstValue(ClaimTypes.Role);
        return role == "1";
    }));
    options.AddPolicy("Customer", policy => policy.RequireAssertion(context =>
    {
        var role = context.User.FindFirstValue(ClaimTypes.Role);
        return role == "2";
    }));
    options.AddPolicy("Guest", policy => policy.RequireAssertion(context =>
    {
        var role = context.User.FindFirstValue(ClaimTypes.Role);
        return role == "3";
    }));
    options.AddPolicy("SpecificUser", policy => policy.RequireAssertion(context =>
    {
        var email = context.User.FindFirstValue(ClaimTypes.Email);
        var userId = context.User.FindFirstValue("Id");
        return (email == "luongfelix14@gmail.com" || userId == "1");
    }));
});
#endregion

#region KetNoiDataBase
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("WebShoppingDatabase"));
            //.UseChangeTrackingProxies();
});
#endregion

#region Session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
#endregion

builder.Services.AddOptions();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IMailServiceSystem, SendMailService>();


builder.Services.AddSignalR(o => o.MaximumReceiveMessageSize = 102400000);
builder.Services.AddSingleton<IVnPayService, VnPayService>();
var app = builder.Build();
app.MapHub<PaymentHubServer>("/paymentserver");
#region CaiDatApp.
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
#endregion

#region ChiaLinkAdminVaKhachHang
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "areaRoute",
        pattern: "{area:exists}/{controller=Authen}/{action=LoginGooglePage}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});
#endregion

#region caiDatThem
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}"
//    );


// G?i DataSeeder ?? thêm d? li?u vào c? s? d? li?u
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        var context = services.GetRequiredService<ApplicationDBContext>();
//        var dataSeeder = new DataSeeder(context);
//        dataSeeder.SeedData();
//    }
//    catch (Exception ex)
//    {
//        var logger = services.GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex, "An error occurred while seeding the database.");
//    }
//}
#endregion

app.Run();
