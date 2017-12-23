using System;

namespace FLive.Web.Models
{
	public class IdentityVerification : Entity
	{
		public string Token { get; set; }
		public string IdentityToken { get; set; }
		public string Email { get; set; }
		public string MobileNumber { get; set; }
		public int? Code { get; set; }
		public DateTime TimeStamp { get; set; }
	}

}