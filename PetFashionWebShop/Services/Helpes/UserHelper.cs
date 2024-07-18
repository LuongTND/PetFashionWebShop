using Data;

namespace PetFashionWebShop.Services.Helpes
{
    public class UserHelper
    {
        public static string GetUserName(int userId, ApplicationDBContext context)
        {
            var user = context.Users.FirstOrDefault(u => u.UserId == userId);
            return user != null ? user.Name : "Unknown";
        }
    }
}
