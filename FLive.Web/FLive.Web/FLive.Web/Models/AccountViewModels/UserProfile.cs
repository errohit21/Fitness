using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FLive.Web.Models.AccountViewModels
{
    public class ProfileViewModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string UserId { get; set; }
        public string Timezone { get; set; }
        public string ProfileImage { get; set; }
        public string PostCode { get; set; }
        public double? LocationLatitude { get; set; }
        public double? LocationLongitude { get; set; }

		public string MobileNumber { get; set; }

		public TrainerViewModel Trainer { get; set; }
        public SubscriberModal User { get; set; }
    }

	public class PasswordResetViewModel
	{
		public string MobileNumber { get; set; }
		public string Email { get; set; }
	}

	public class PasswordResetCodeVerificationViewModel
	{
		public string MobileNumber { get; set; }
		public string Email { get; set; }
		public int Code { get; set; }
		public string Token { get; set; }
	}
	public class PasswordSetRequest
	{
		public string Token { get; set; }
		public string Email { get; set; }
	}

	public class PushNotificationsRegisterModel
	{
		public string DeviceToken { get; set; }
		public string Platform { get; set; }
	}



	public class PasswordSetViewModel
	{
		public string MobileNumber { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string ConfirmPassword { get; set; }
		public string Token { get; set; }
	}
}
