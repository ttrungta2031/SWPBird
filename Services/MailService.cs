using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.IO;
using System.Threading.Tasks;
using SWPBirdBoarding.Models;
using SWPBirdBoarding.ImplementServices;

namespace SWPBirdBoarding.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendMail(MailRequest mailContent)
        {
            var email = new MimeMessage();
            email.Sender = new MailboxAddress("SWPBirdBoarding System", "servicemailbybiv@gmail.com");
            email.From.Add(new MailboxAddress("SWPBirdBoarding System", "servicemailbybiv@gmail.com"));
            email.To.Add(MailboxAddress.Parse(mailContent.ToEmail));
            email.Subject = mailContent.Subject;


            var builder = new BodyBuilder();
            builder.HtmlBody = mailContent.Description;
            email.Body = builder.ToMessageBody();

            // dùng SmtpClient của MailKit
            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate("servicemailbybiv@gmail.com", "gwvclovpuphcecms");
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
                System.IO.Directory.CreateDirectory("mailssave");
                var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                await email.WriteToAsync(emailsavefile);
            }

            smtp.Disconnect(true);

 

        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage, string value)
        {
            await SendMail(new MailRequest()
            {
                ToEmail = email,
                Subject = subject,
                Description = htmlMessage,
                Value = value
            });
        }









        public async Task SendCodeEmailAsync(MailRequest mailrequest)
        {
            var logo = "https://i1.createsend1.com/ei/y/94/997/003/223954/csfinal/looka.com_editor_77920870121-6a86671ce57a11b8.png";
            var username = mailrequest.ToEmail.Split('@')[0];
            string FilePath = Directory.GetCurrentDirectory() + "\\Templates\\MailTemplate.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            MailText = MailText.Replace("[subject]", mailrequest.Subject).
                Replace("[description]", mailDescription(mailrequest.Description)).
                Replace("[value]", mailValue(mailrequest.Value)).
                Replace("[date]", DateTime.Today.ToString("D")).
                Replace("[username]", username).
                Replace("[logo]", logo);
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse("fptpsycteam@gmail.com");
            email.To.Add(MailboxAddress.Parse(mailrequest.ToEmail));
            email.Subject = $"{mailrequest.Subject}";
            var builder = new BodyBuilder();
            builder.HtmlBody = MailText;
            email.Body = builder.ToMessageBody();
           SmtpClient smtp = new SmtpClient();
            
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("fptpsycteam@gmail.com", "kqrryzyfzzuqodxs");           
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        } 























        public string mailDescription(string s) 
        {
            if (s == "sendCodeEmail")
            {
                return "You are one step away.<br>Enter the code to verify for your [username] account on <span style = \"color:#2eb2d5;\" >Psychological Couselling </ span >";
            }
            return "";
        }
        public string mailValue(string s)
        {
            if (s != "")
            {
                return s;
            }
            return "";
        }
    }
}
