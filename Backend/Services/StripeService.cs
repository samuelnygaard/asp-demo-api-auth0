namespace Backend.Services
{
    public class StripeService
    {
        private readonly StripeClient stripeClient;
        public readonly string WebhookSigningKey;

        public StripeService(IConfiguration configuration)
        {
            this.stripeClient = new StripeClient(configuration["Stripe:SecretKey"], configuration["Stripe:PublicKey"]);
            this.WebhookSigningKey = configuration["Stripe:WebhookSigningKey"];
        }

        public Session CreateSession(string domain, string? stripeCustomerId)
        {
            var options = new SessionCreateOptions

            {
                Customer = stripeCustomerId,

                LineItems = new List<SessionLineItemOptions>

                {

                  new SessionLineItemOptions

                  {
                    // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                    //Price = "price_1K4CQJKpm6ssj49857TRzn6n",
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        Currency = "eur",
                        //Product = "prod_KjflM2LsoYRrS9",
                        ProductData = new SessionLineItemPriceDataProductDataOptions() {
                            Name = "Blog post demo",
                            Description = "Dummy Description"
                        },
                        UnitAmount = 12500
                    }

                  },

                },

                Mode = "payment",

                SuccessUrl = domain + "/success.html",

                CancelUrl = domain + "/cancel.html",

                // AutomaticTax = new SessionAutomaticTaxOptions { Enabled = true },

            };

            var service = new SessionService(stripeClient);

            return service.Create(options);
        }
    }
}
