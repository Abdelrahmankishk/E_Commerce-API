using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Basketd;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace E_Commerce.API.Controllers;

public class PaymentsController : ApiBaseController
{
    private readonly IPaymentService paymentService;
    private readonly PaymentGatwaySettings paymentGatwaySettings;

    public PaymentsController(IPaymentService paymentService,IOptions<PaymentGatwaySettings> options)
    {
        this.paymentService = paymentService;
        paymentGatwaySettings = options.Value;
    }
    [Authorize]
    [HttpPost("{basketId}")]
    public async Task<ActionResult<BasketDto>> CreateOrUpdatePaymentIntent(string basketId,CancellationToken ct)
    {
        return ToActionResult(await paymentService.CreateOrUpdatePaymentIntentAsync(basketId, ct));
    }
    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var requestJson = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var StripeEvent = EventUtility.ConstructEvent(requestJson,
                Request.Headers["Stripe-Signature"],
                paymentGatwaySettings.WebHookSecret);

            switch (StripeEvent.Type)
            {
                case EventTypes.PaymentIntentSucceeded:
                    var paymentIntent = StripeEvent.Data.Object as PaymentIntent;
                    if (paymentIntent is not null)
                        await paymentService.PaymentSucceed(paymentIntent.Id);
                    break;

                case EventTypes.PaymentIntentPaymentFailed:
                    var paymentIntentFailed = StripeEvent.Data.Object as PaymentIntent;
                    if (paymentIntentFailed is not null)
                        await paymentService.PaymentFailed(paymentIntentFailed.Id);
                    break;
                default:
                    break;
            }

            return Ok();

        }
        catch (StripeException ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest(ex.Message);
        }
    }
}
