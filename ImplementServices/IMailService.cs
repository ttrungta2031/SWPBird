using System.Threading.Tasks;
using SWPBirdBoarding.Models;

namespace SWPBirdBoarding.ImplementServices
{
    public interface IMailService 
    {
        Task SendCodeEmailAsync(MailRequest mailRequest);
        Task SendEmailAsync(string email, string subject, string htmlMessage, string value);
    }
}
