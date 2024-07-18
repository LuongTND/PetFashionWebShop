using PetFashionWebShop.ModelServices;

namespace PetFashionWebShop.Services.Email
{
    public interface IMailServiceSystem
    {
        Task SendMail(MailContent mailContent);

        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }

}
