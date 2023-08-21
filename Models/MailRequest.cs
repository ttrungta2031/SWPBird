using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SWPBirdBoarding.Models
{
    public class MailRequest 
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
}
