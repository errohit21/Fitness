using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace FLive.Web.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public UserType UserType { get; set; }
        public string Name { get; set; }
        public string DeviceToken { get; set; }
        public string FacebookToken { get; set; }
        public string Platform { get; set; }
        public string Timezone { get; set; }
        public string ProfileImage { get; set; }
		public string MobileNumber { get; set; }
    }

	public class EmailConfirmationRequestModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

	}
	public class EmailVerificationModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }
		public string Token { get; set; }
		public int Code { get; set; }

	}
}