using System.Threading.Tasks;

namespace FLive.Web.Services
{
    public interface IPaymentGatewayServie
    {
        Task Charge(long subscriberId);
    }
}