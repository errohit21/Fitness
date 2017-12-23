using System.Threading.Tasks;

namespace FLive.Web.Services
{
	public class PaymentGatewayServie : IPaymentGatewayServie
    {
        public async Task Charge(long subscriberId)
        {
            //do this via a queue to guarantee the reliability
            throw new System.NotImplementedException();
        }
    }
}