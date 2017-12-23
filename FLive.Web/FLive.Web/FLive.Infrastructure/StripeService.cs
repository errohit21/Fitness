using System;
using System.Configuration;
using System.Threading.Tasks;
using Stripe;

namespace FLive.Infrastructure
{
    public class StripeService
    {
        private readonly string _dbConnection;

        public StripeService(string dbConnection)
        {
            _dbConnection = dbConnection;

            StripeConfiguration.SetApiKey(ConfigurationManager.AppSettings["Stripe.SecretKey"]);
        }

        public async Task<Tuple<bool, string>> TryCharge(string customerId, int amount, string currency)
        {
            try
            {
                var chargeCreateOptions = new StripeChargeCreateOptions
                {
                    CustomerId = customerId,
                    Amount = amount,
                    Currency = currency
                };

                var chargeService = new StripeChargeService();

                chargeService.Create(chargeCreateOptions);
              
            }
            catch (Exception exception)
            {
                return Tuple.Create(false, exception.Message);
            }
            return Tuple.Create(true, "success!");
            ;
        }

        public async Task<Tuple<bool, string>> TryChargeUsingConnet(string customerId,string userId, int amount, string currency)
        {
            try
            {
                var applicationFeePercentage =
                    Convert.ToInt32(ConfigurationManager.AppSettings["ApplicationFeePercentage"]);
                var applicationFee = (amount/100)* applicationFeePercentage;
                var chargeCreateOptions = new StripeChargeCreateOptions
                {
                    CustomerId = customerId,
                    Amount = amount,
                    Currency = currency,
                    Destination = userId,
                    ApplicationFee = applicationFee
                };

                var chargeService = new StripeChargeService();

                chargeService.Create(chargeCreateOptions);

            }
            catch (Exception exception)
            {
                return Tuple.Create(false, exception.Message);
            }
            return Tuple.Create(true, "success!");
            ;
        }

    }
}