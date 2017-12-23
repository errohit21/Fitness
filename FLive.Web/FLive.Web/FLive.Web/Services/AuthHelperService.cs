using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FLive.Web.Data;
using Microsoft.EntityFrameworkCore;
using FLive.Web.Models;
using FLive.Web.Models.AccountViewModels;
using FLive.Web.Repositories;
using FLive.Web.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace FLive.Web.Services
{

	public class AuthHelperService : IAuthHelperService
	{
		private readonly IGenericRepository<IdentityVerification> _passwordResetRequest;
		private readonly ISmsSender _smsSender;
		private readonly IEmailSender _emailSender;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly EmailSettings _emailOptions;

		public AuthHelperService(IGenericRepository<IdentityVerification> passwordResetRequest, ISmsSender smsSender, IEmailSender emailSender, UserManager<ApplicationUser> userManager, IOptions<EmailSettings> emailOptions)
		{
			_passwordResetRequest = passwordResetRequest;
			_smsSender = smsSender;
			_emailSender = emailSender;
			_userManager = userManager;
			_emailOptions = emailOptions.Value;

		}
		public async Task<IdentityVerification> RequestPasswordReset(PasswordResetViewModel resetViewModel)
		{

			var user = await FindUser(resetViewModel.Email, resetViewModel.MobileNumber);

			if (user == null)
				throw new BusinessException("Couldn't find a user with the passed in mobile number or email");

			var identityToken = await _userManager.GeneratePasswordResetTokenAsync(user);

			var passwordResetRequest = new IdentityVerification
			{
				Code = GenerateRandomNumber(),
				Email = user.Email,
				MobileNumber = user.MobileNumber,
				Token = Guid.NewGuid().ToString(),
				IdentityToken = identityToken,
				TimeStamp = DateTime.UtcNow
			};
			await _passwordResetRequest.Add(passwordResetRequest);

			var message = $"Your FLIVE Password reset code is : {passwordResetRequest.Code}";
			var subject = $"FLIVE Password reset code";

			var parameters = new Dictionary<string, string>();
			parameters.Add("-pin-", passwordResetRequest.Code.ToString());

			if (resetViewModel.Email != null)
				await _emailSender.SendEmailAsync(resetViewModel.Email, subject, _emailOptions.PasswordResetTemplateId, parameters);
			if (resetViewModel.MobileNumber != null)
				await _smsSender.SendSmsAsync(resetViewModel.MobileNumber, message);

			passwordResetRequest.Code = null;
			return passwordResetRequest;

		}

		public async Task<IdentityVerification> SendEmailConfirmation(string email)
		{
			var user1 = await FindUser(email, null);
			if (user1.IsNotNull())
				throw new BusinessException("email already used");

			var emailConfirmation = new IdentityVerification
			{
				Code = GenerateRandomNumber(),
				Email = email,
				MobileNumber = "",
				Token = Guid.NewGuid().ToString(),
				IdentityToken = null,
				TimeStamp = DateTime.UtcNow
			};
			await _passwordResetRequest.Add(emailConfirmation);

			var subject = $"FLIVE email confirmation code";
			var parameters = new Dictionary<string, string>();
			parameters.Add("-pin-", emailConfirmation.Code.ToString());

			await _emailSender.SendEmailAsync(email, subject, _emailOptions.EmailConfirmationTemplateId, parameters);
			return emailConfirmation;
		}

		public int GenerateRandomNumber()
		{
			int _min = 1000;
			int _max = 9999;
			Random _rdm = new Random();
			return _rdm.Next(_min, _max);
		}

		public async Task<PasswordSetRequest> VerifyResetCode(PasswordResetCodeVerificationViewModel passwordResetVerificationViewModel)
		{
			var match = await _passwordResetRequest.GetFirstOrDefault(a => a.Token == passwordResetVerificationViewModel.Token && a.Code == passwordResetVerificationViewModel.Code);

			if (match == null)
				throw new BusinessException("Couldn't validate the password reset token with the code entered");
			var user = await FindUser(passwordResetVerificationViewModel.Email, passwordResetVerificationViewModel.MobileNumber);

			if (user == null)
				throw new BusinessException("Couldn't find a user with the passed in mobile number or email");

			return new PasswordSetRequest { Email = user.Email, Token = passwordResetVerificationViewModel.Token };
		}

		public async Task VerifyEmailCode(EmailVerificationModel registrationModel)
		{
			var match = await _passwordResetRequest.GetFirstOrDefault(a => a.Token == registrationModel.Token && a.Code == registrationModel.Code);

			if (match == null)
				throw new BusinessException("Couldn't validate");

			await _passwordResetRequest.Remove(match);
		}


		private async Task<ApplicationUser> FindUser(string email, string mobile)
		{
			if (email.IsNotNull())
				return await _userManager.FindByEmailAsync(email);

			var users = await _passwordResetRequest.Context.Users.Where(a => a.MobileNumber == mobile).ToListAsync();

			return users.FirstOrDefault();
		}
		public async Task SetPassword(PasswordSetViewModel passwordSetViewModel)
		{
			var resetRequest = await _passwordResetRequest.GetFirstOrDefault(a => a.Token == passwordSetViewModel.Token);
			if (resetRequest == null)
				throw new BusinessException("Couldn't find the reset token");
			var user = await FindUser(passwordSetViewModel.Email, passwordSetViewModel.MobileNumber);
			if (user == null)
				throw new BusinessException("Couldn't find a user with the passed in mobile number or email");
			if (passwordSetViewModel.Password != passwordSetViewModel.ConfirmPassword)
				throw new BusinessException("Passwords don't match");

			var result = await _userManager.ResetPasswordAsync(user, resetRequest.IdentityToken, passwordSetViewModel.Password);

			if (result.Succeeded)
				await _passwordResetRequest.Remove(resetRequest);
			else
				throw new BusinessException("Failed to reset password");

		}
	}

}
