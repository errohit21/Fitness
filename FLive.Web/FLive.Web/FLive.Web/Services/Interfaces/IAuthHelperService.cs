using System.Collections.Generic;
using System.Threading.Tasks;
using FLive.Web.Models;
using FLive.Web.Models.AccountViewModels;
using FLive.Web.Models.ApplicationViewModels;

namespace FLive.Web.Services
{
    public interface IAuthHelperService
	{
		Task<IdentityVerification> RequestPasswordReset(PasswordResetViewModel resetViewModel);
		Task<PasswordSetRequest> VerifyResetCode(PasswordResetCodeVerificationViewModel passwordResetVerificationViewModel);
		Task SetPassword(PasswordSetViewModel passwordSetViewModel);
		Task<IdentityVerification> SendEmailConfirmation(string email);
		Task VerifyEmailCode(EmailVerificationModel registrationModel);
	}
}