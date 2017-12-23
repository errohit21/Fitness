using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using FLive.Web.Models;
using Microsoft.Extensions.Options;
using FLive.Web.Shared;
using System.Collections.Generic;

namespace FLive.Web.Services
{
	public class EmailSender : IEmailSender
	{
		private EmailSettings _emailSettings;
		public EmailSender(IOptions<EmailSettings> emailOptions)
		{
			_emailSettings = emailOptions.Value;
		}
		public async Task SendEmailAsync(string email, string subject, string templateId, Dictionary<string,string> parameters)
		{

			var client = new SendGridClient(_emailSettings.ApiKey);
			var msg = new SendGridMessage();
			msg.SetFrom(new EmailAddress(_emailSettings.FromEmail, _emailSettings.FromEmail));
			msg.SetSubject(subject);
			msg.AddTo(new EmailAddress(email, email));

			msg.SetTemplateId(templateId);

			foreach (var parameter in parameters)
			{
				msg.AddSubstitution(parameter.Key, parameter.Value);

			}

			var response = await client.SendEmailAsync(msg);
		}


	}
}