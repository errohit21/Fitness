using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FLive.Web.Models
{
	public class Settings : Entity
    {
        public string PrivacyPolicy { get; set; }
        public string UserTermsAndConditions { get; set; }
        public string TrainerTermsAndConditions { get; set; }
        public string Support { get; set; }
		[NotMapped]
		public string[] Countries { get; set; }
		[NotMapped]
		public IEnumerable<Country> CountryList { get; set; }
	}

	public class SmsSettings
	{
		public string Sid { get; set; }
		public string Token { get; set; }
		public string BaseUri { get; set; }
		public string RequestUri { get; set; }
		public string From { get; set; }
	}

	public class EmailSettings
	{
		public string ApiKey { get; set; }
		public string FromEmail { get; set; }
		public string FromName { get; set; }
		public string PasswordResetTemplateId { get; set; }
		public string EmailConfirmationTemplateId { get; set; }
		public string FliveNotificationEmail { get; set; }
		public string RegistrationNotificationTemplateId { get; set; }
		

	}
}