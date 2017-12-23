using FLive.Web.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FLive.Web.Services
{
	public class SmsSender : ISmsSender
	{
		private readonly SmsSettings _smsSettings;
		public SmsSender(IOptions<SmsSettings> smsSettings)
		{
			_smsSettings = smsSettings.Value;

		}
		public async Task SendSmsAsync(string number, string message)
		{
			using (var client = new HttpClient { BaseAddress = new Uri(_smsSettings.BaseUri) })
			{
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
					Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_smsSettings.Sid}:{_smsSettings.Token}")));

				var content = new FormUrlEncodedContent(new[]
				{
			new KeyValuePair<string, string>("To",$"+{number}"),
			new KeyValuePair<string, string>("From", _smsSettings.From),
			new KeyValuePair<string, string>("Body", message)
		});

				await client.PostAsync(_smsSettings.RequestUri, content).ConfigureAwait(false);
			}
		}

	}


	
}