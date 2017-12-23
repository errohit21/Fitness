using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Model;

namespace FLive.Web.Services
{
	public interface ISmsSender
	{
		Task SendSmsAsync(string number, string message);
	}
}