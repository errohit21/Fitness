using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using NUnit.Framework;
using Stripe;

namespace FLive.Infrastructure
{
    [TestFixture]
    public class PaymentsWorkerTest
    {
     
        [Test]
        public async Task test_payments()
        {
            StripeConfiguration.SetApiKey(ConfigurationManager.AppSettings["Stripe.SecretKey"]);

            var stripeTokenreateOptions = new StripeTokenCreateOptions();

            stripeTokenreateOptions.Card = new StripeCreditCardOptions()
            {
                Number = "4242424242424242",
                ExpirationYear = "2022",
                ExpirationMonth = "10",
            };

            var tokenService = new StripeTokenService();
            StripeToken stripeToken = tokenService.Create(stripeTokenreateOptions);

            StripeCustomerCreateOptions customerCreateOptions = new StripeCustomerCreateOptions
            {
                SourceToken = stripeToken.Id,
                Email = $"{Guid.NewGuid()}@test.com"
            };

            var customerService = new StripeCustomerService();
            var customer = customerService.Create(customerCreateOptions);

            var dbConnection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            var stripeService = new StripeService(dbConnection);
            await stripeService.TryCharge(customer.Id, 300, "AUD");

        }

        [Test]
        public async Task try_charge_should_not_throw_exceptions()
        {
            var dbConnection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            PaymentsWorker paymentsWorker = new PaymentsWorker(dbConnection,"","");
            paymentsWorker.TryChargeCompletedWorkouts().Wait();
        }

        [Test]
        public async Task try_payments_with_stripe_connect()
        {
            var dbConnection = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            var paymentsWorker = new StripeService(dbConnection);
            await paymentsWorker.TryChargeUsingConnet("cus_AE3vffag7fTaVc", "acct_19tbtZDSMoDEiFde", 1000, "AUD");

        }

    }
}