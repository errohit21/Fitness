using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;


namespace FLive.Web.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject,string templateId, Dictionary<string, string> parameters);
    }

	
}
