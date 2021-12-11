using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Backend.Controllers
{
    /// <summary>
    /// WeatherForecastController test
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> log;
        private readonly StripeService stripeService;

        public PaymentController(ILogger<WeatherForecastController> logger, StripeService _stripeService)
        {
            this.log = logger;
            this.stripeService = _stripeService;
        }
        

        /// <summary>
        /// Test
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            log.LogInformation("Stripe Get");

            var referer = Request.Headers["Referer"].ToString().ToLower();

            var session = stripeService.CreateSession(referer, null);

            return Ok(new { session.Url });
        }

        /// <summary>
        /// Test
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("Link")]
        public async Task<IActionResult> GetLink()
        {
            log.LogInformation("Stripe Get");

            var referer = Request.Headers["Referer"].ToString().ToLower();

            var link = stripeService.CreateAccountLink(referer, User.GetEmail());

            return Ok(new { link.Url });
        }

        /// <summary>
        /// Test
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Webhook")]
        public async Task<IActionResult> Webhook()
        {
            log.LogInformation("Webhook");

            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], stripeService.WebhookSigningKey);

                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                }
                // ... handle other event types
                else
                {
                    log.LogInformation("Unhandled event type: {0}", stripeEvent.Type);
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
    }
}
