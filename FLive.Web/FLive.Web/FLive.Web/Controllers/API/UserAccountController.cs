using System;
using System.Text;
using System.Threading.Tasks;
using FLive.Web.Models;
using FLive.Web.Models.AccountViewModels;
using FLive.Web.Repositories;
using FLive.Web.Security;
using FLive.Web.Services;
using FLive.Web.Services.Interfaces;
using FLive.Web.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Slack;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace FLive.Web.Controllers.API
{
	[Route("api/[controller]")]
	[Authorize]
	public class UserAccountController : Controller
	{
		private readonly IEmailSender _emailSender;
		private readonly Microsoft.Extensions.Logging.ILogger _logger;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly ISmsSender _smsSender;
		private readonly ISubscriberService _subscriberService;
		private readonly ITrainerService _trainerService;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IAzurePushNotificationService _azurePushNotificationService;
		private readonly IFileUploadRepository _fileUploadRepository;
		private readonly IAuthHelperService _authHelperService;
		private readonly EmailSettings _emailSettings;

		public UserAccountController(UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			IEmailSender emailSender,
			ISmsSender smsSender,
			ILoggerFactory loggerFactory,
			ITrainerService trainerService,
			ISubscriberService subscriberService, IAzurePushNotificationService azurePushNotificationService,
			IFileUploadRepository fileUploadRepository, 
            IAuthHelperService authHelperService, 
            IOptions<EmailSettings> emailSettings)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_emailSender = emailSender;
			_smsSender = smsSender;
			_logger = loggerFactory.CreateLogger<AccountController>();
			_trainerService = trainerService;
			_subscriberService = subscriberService;
			_azurePushNotificationService = azurePushNotificationService;
			_fileUploadRepository = fileUploadRepository;
			_authHelperService = authHelperService;
			_emailSettings = emailSettings.Value;

		}

		[HttpPost("register")]
		[AllowAnonymous]
		public async Task RegisterTrainer([FromBody] RegisterViewModel registrationModel)
		{
			var applicationUser = await RegisterAndGetApplicationUser(registrationModel);

			await HandlePostRegistration(registrationModel, applicationUser);
		}

		[HttpPost("emailconfirmation")]
		[AllowAnonymous]
		public async Task<IdentityVerification> SendEmailConfirmation([FromBody] EmailConfirmationRequestModel registrationModel)
		{
			return await _authHelperService.SendEmailConfirmation(registrationModel.Email);
		}

		[HttpPost("verifyemail")]
		[AllowAnonymous]
		public async Task VerifyEmailCode([FromBody] EmailVerificationModel registrationModel)
		{
			await _authHelperService.VerifyEmailCode(registrationModel);
		}


		[HttpPost("registerwithfacebook")]
		[AllowAnonymous]
		public async Task ExternalRegister([FromBody] RegisterViewModel registrationModel)
		{
			var facebookMeResult = await FacebookAuthHelper.VerifyAccessToken(registrationModel.Email, registrationModel.FacebookToken);

			var existinguser = await _userManager.FindByEmailAsync(facebookMeResult.Email);
			if (existinguser.IsNotNull())
				throw new BusinessException($"A user with email {existinguser.Email} already exists!");
			var applicationUser = await ExternalRegisterAndGetApplicationUser(registrationModel);

			await HandlePostRegistration(registrationModel, applicationUser);
		}



		[HttpGet("profile")]
        //[Authorize]
        [AllowAnonymous]
        public async Task<ProfileViewModel> GetUserProfile()
		{
			return await BuildAndReturnUserProfile();
		}



        [HttpGet("subscribedtrainers")]
        //[Authorize]
        [AllowAnonymous]
        public async Task<IEnumerable<Trainer>> GetSubscribedTrainers()
        {
            var subscriber = await _subscriberService.GetByUserId(User.GetUserId());
           return await  _trainerService.GetSubscribedTrainers(subscriber.Id);
        }



        [HttpPost("passwordreset")]
		[AllowAnonymous]
		public async Task<IdentityVerification> PasswordReset([FromBody]PasswordResetViewModel passwordResetModel)
		{
			return await _authHelperService.RequestPasswordReset(passwordResetModel);
		}

		[HttpPost("verifycode")]
		[AllowAnonymous]
		public async Task<PasswordSetRequest> VerifyPasswordResetCode([FromBody]PasswordResetCodeVerificationViewModel passwordResetVerificationViewModel)
		{
			return await _authHelperService.VerifyResetCode(passwordResetVerificationViewModel);
		}

		[HttpPost("setpassword")]
		[AllowAnonymous]
		public async Task SetPassword([FromBody]PasswordSetViewModel passwordSetModel)
		{
			await _authHelperService.SetPassword(passwordSetModel);
		}

		[HttpPost("registerforpushnotifications")]
		public async Task RegisterForPushNotifications([FromBody]PushNotificationsRegisterModel pushNotificationsModel)
		{
			var user = await _userManager.FindByIdAsync(User.GetUserId());
			user.DeviceToken = pushNotificationsModel.DeviceToken;
			user.Platform = pushNotificationsModel.Platform;

			await _azurePushNotificationService.Register(user);
		}

		[HttpPost("unregisterfrompushnotifications")]
		public async Task UnRegisterFromPushNotifications([FromBody]PushNotificationsRegisterModel pushNotificationsModel)
		{
			var user = await _userManager.FindByIdAsync(User.GetUserId());
			user.DeviceToken = pushNotificationsModel.DeviceToken;
			user.Platform = pushNotificationsModel.Platform;

			await _azurePushNotificationService.UnRegister(user);
		}



		[HttpGet("profile/trainer/{trainerId}")]
		[AllowAnonymous]
		public async Task<ProfileViewModel> GetTrainerProfile(long trainerId)
		{
			var trainer = await _trainerService.Get(trainerId);
			var subscriber = await _subscriberService.GetByUserId(User.GetUserId());

			if (trainer.IsNull())
				throw new BusinessException("Not found");
			var userProfile = new ProfileViewModel
			{
				Age =Convert.ToInt16( trainer.ApplicationUser.Age),
				Name = trainer.ApplicationUser.Name,
				ProfileImage = trainer.ApplicationUser.ProfileImageUrl,
				Trainer = await _trainerService.GetTrainerProfile(trainer.ApplicationUser.Id, subscriber.IsNotNull() ? subscriber.Id : (long?)null)
			};
			return userProfile;
		}


		private async Task<ProfileViewModel> BuildAndReturnUserProfile()
		{
			var user = await _userManager.FindByIdAsync(User.GetUserId());
			bool hasTrainerProfile = user.UserType == UserType.Trainer || user.UserType == UserType.Both;
			bool hasUserProfile = user.UserType == UserType.Subscriber || user.UserType == UserType.Both;

			var userProfile = new ProfileViewModel
			{
                //Age = user.Age,
                Age =Convert.ToInt16(user.Age),
                UserId = user.Id,
				Name = user.Name,
				MobileNumber = user.MobileNumber,
				ProfileImage = user.ProfileImageUrl,
				PostCode = user.PostCode,
				LocationLatitude = user.LocationLatitude,
				LocationLongitude = user.LocationLongitude,
				Timezone = user.Timezone,
				Trainer = hasTrainerProfile ? await _trainerService.GetTrainerProfile(user.Id) : null,
				User = hasUserProfile ? await _subscriberService.GetSubscriberProfile(user.Id) : null,


			};
			return userProfile;
		}


		[HttpPost("profile")]
		[Authorize]
		public async Task ProfileUpdate([FromBody] ProfileViewModel profilePostViewModel)
		{
			if (profilePostViewModel.Timezone.IsNull())
				throw new BusinessException($"Timezone must be supplied");

			await UpdateUser(profilePostViewModel);

			if (profilePostViewModel.Trainer.IsNotNull())
				await _trainerService.UpdateTrainer(profilePostViewModel);
			if (profilePostViewModel.User.IsNotNull())
				await _subscriberService.UpdateSubscriber(profilePostViewModel);
		}


		[HttpPost("profileimage")]
		[Produces("application/json")]
		[Consumes("multipart/form-data", "application/json-patch+json", "application/json")]
		public async Task<ProfileViewModel> UploadPreview(IFormFile file)
		{
			var uploadedUrl = "";

			var stream = file.OpenReadStream();
			uploadedUrl = await _fileUploadRepository.UploadFileAsBlob(stream, file.FileName, "profileimages");

			var user = await _userManager.FindByIdAsync(User.GetUserId());
			user.ProfileImageUrl = uploadedUrl;
			await _userManager.UpdateAsync(user);
			return await BuildAndReturnUserProfile();
		}


		private async Task UpdateUser(ProfileViewModel profilePostViewModel)
		{
			var user = await _userManager.FindByIdAsync(User.GetUserId());
			if (user.IsNull())
				throw new BusinessException($"User with Id{User.GetUserId()}");

			profilePostViewModel.UserId = user.Id;
            //user.Age = profilePostViewModel.Age;
            user.Age = profilePostViewModel.Age.ToString();
            user.Name = profilePostViewModel.Name;
			user.PostCode = profilePostViewModel.PostCode;
			user.LocationLatitude = profilePostViewModel.LocationLatitude;
			user.LocationLongitude = profilePostViewModel.LocationLongitude;
			if (profilePostViewModel.ProfileImage.IsNotNull())
				user.ProfileImageUrl = profilePostViewModel.ProfileImage;
			user.Timezone = profilePostViewModel.Timezone;

			await _userManager.UpdateAsync(user);

		}


		private async Task HandlePostRegistration(RegisterViewModel registrationModel, ApplicationUser applicationUser)
		{

			switch ((int)registrationModel.UserType)
			{
				case (int)UserType.Trainer:
					await _trainerService.CreateTrainer(applicationUser, registrationModel.DeviceToken);

					break;
				case (int)UserType.Subscriber:
					await _subscriberService.CreateSubscriber(applicationUser, registrationModel.DeviceToken);
					break;
				case (int)UserType.Both:
					await _trainerService.CreateTrainer(applicationUser, registrationModel.DeviceToken);
					await _subscriberService.CreateSubscriber(applicationUser, registrationModel.DeviceToken);
					break;
			}


		}



		private async Task<ApplicationUser> RegisterAndGetApplicationUser(RegisterViewModel registrationModel)
		{
			IdentityResult identityResult = null;

			var user = new ApplicationUser
			{
				UserName = registrationModel.Email,
				Email = registrationModel.Email,
				DeviceToken = registrationModel.DeviceToken,
				UserType = registrationModel.UserType,
				Platform = registrationModel.Platform,
				Name = registrationModel.Name,
				Timezone = registrationModel.Timezone,
				MobileNumber = registrationModel.MobileNumber
			};

			identityResult = await _userManager.CreateAsync(user, registrationModel.Password);


			await ValidateAndProceeed(identityResult, user);
			await _azurePushNotificationService.Register(user);

			var applicationUser = await _userManager.FindByEmailAsync(user.Email);
			return applicationUser;
		}

		private async Task<ApplicationUser> ExternalRegisterAndGetApplicationUser(RegisterViewModel registrationModel)
		{
			IdentityResult identityResult = null;

			var user = new ApplicationUser
			{

				UserName = registrationModel.Email,
				Email = registrationModel.Email,
				MobileNumber = registrationModel.MobileNumber,
				DeviceToken = registrationModel.DeviceToken,
				UserType = UserType.Both,
				Platform = registrationModel.Platform,
				Name = registrationModel.Name,
				FacebookToken = registrationModel.FacebookToken,
				ProfileImageUrl = registrationModel.ProfileImage
			};


			//user.Id = User.GetUserId();

			identityResult = await _userManager.CreateAsync(user);
			await ValidateAndProceeed(identityResult, user);
			await _azurePushNotificationService.Register(user);

			var applicationUser = await _userManager.FindByEmailAsync(user.Email);
			return applicationUser;
		}




		private async Task ValidateAndProceeed(IdentityResult result, ApplicationUser user)
		{
			if (result.Succeeded)
			{
				var userType = (user.UserType == UserType.Both || user.UserType == UserType.Trainer) ? "Trainer" : "User";
				var parameters = new Dictionary<string, string>();

				parameters.Add("-UserType-", userType);
				parameters.Add("-Name-", user.Name);
				parameters.Add("-TrainerEmail-", user.Email);
				parameters.Add("-RegistrationTime-", DateTime.UtcNow.ToString());

				//await _emailSender.SendEmailAsync(_emailSettings.FliveNotificationEmail, $"New Registration ({userType})", _emailSettings.RegistrationNotificationTemplateId, parameters);

				//await _signInManager.SignInAsync(user, false);
				_logger.LogInformation(3, "User created a new account with password.");
			}
			else
			{
				var errors = GetAllErrors(result);
				_logger.LogError(errors);
				throw new BusinessException(errors);
			}
		}

		private string GetAllErrors(IdentityResult result)
		{
			var sb = new StringBuilder();
			foreach (var error in result.Errors)
			{
				sb.AppendLine(error.Description);
			}
			return sb.ToString();
		}
	}
}