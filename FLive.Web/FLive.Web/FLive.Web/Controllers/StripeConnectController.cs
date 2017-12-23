using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FLive.Web.Services;
using FLive.Web.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace FLive.Web.Controllers
{
    //[Authorize]
    public class StripeConnectController : Controller
    {
        private readonly ITrainerService _trainerService;
        private readonly StripeConfig _stripeConfig;
        private readonly ILogger _logger;

        public StripeConnectController(ITrainerService trainerService, IOptions<StripeConfig> stripeConfig)
        {
            _trainerService = trainerService;
            _stripeConfig = stripeConfig.Value;
            //_logger = logger;
        }

        public async Task<IActionResult> Index(long trainerId,string accessToken)
        {

            var trainer = await _trainerService.Get(trainerId);
            var redirectUrl =
                $"https://connect.stripe.com/oauth/authorize?response_type=code&client_id={_stripeConfig.ClientId}&scope=read_write&state={trainer.ApplicationUser.Id}";
            ViewData["StripeConnectUrl"] = redirectUrl;
            return View();
        }

        public async Task<IActionResult> Callback()
        {
            var code = Request.Query["code"];
            var state = Request.Query["state"];
            var scope = Request.Query["scope"];
            var error = Request.Query["error"].ToString();

            var error_description = Request.Query["error_description"];

            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogError($"{scope}:{error}:{error_description}");
                return RedirectToAction("Error");

            }
            else
            {
                var trainer = await _trainerService.GetTrainerByAspnetUserId(state);
                HttpClient httpClient = new HttpClient();

                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_stripeConfig.SecretKey}");

                var url =
                    $"https://manage.stripe.com/oauth/token?grant_type=authorization_code&client_id={_stripeConfig.ClientId}&code={code}";
                var result =
              await
                    httpClient.PostAsync(url, new StringContent(""));
                var jsonResult = await result.Content.ReadAsStringAsync();

                dynamic d = JObject.Parse(jsonResult);

                var stripeUserId =  d.stripe_user_id.ToString();
                await _trainerService.UpdateStripeToken(trainer.Id, stripeUserId, jsonResult);
                return RedirectToAction("Success");
            }
        }

        private  string BuildPostModel(string code)
        {
            return $@"{{""client_secret"":""{_stripeConfig.SecretKey}"",""code"":""{code}"",""grant_type"":""authorization_code"",""client_id"":""{_stripeConfig.ClientId}""}}";
        }

        public IActionResult Success()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View();
        }

    }
}
