using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using Microsoft.Extensions.Options;
using Stripe;

namespace E_Commerce.Infrastructure.Payments;

internal class StripePaymentGateway : IPaymentGateway
{
    private readonly PaymentGatwaySettings _payment;
    private readonly PaymentIntentService _paymentIntentService = new();
    public StripePaymentGateway(IOptions<PaymentGatwaySettings> options)
    {
        _payment = options.Value;
        StripeConfiguration.ApiKey = options.Value.SecretKey;
    }
    public async Task<PaymentIntentResult> CreatePaymentIntentAsync(decimal amount, string currenct, CancellationToken ct = default)
    {
        var options = new PaymentIntentCreateOptions()
        {
            Amount = (long)amount,
            Currency = currenct.ToLower(),
            AllowedPaymentMethodTypes = ["card"]
        };
        var intent =  await _paymentIntentService.CreateAsync(options, cancellationToken: ct);
        return new PaymentIntentResult(intent.Id, intent.ClientSecret);
    }

    public async Task<PaymentIntentResult> UpdatePaymentIntentAsync(decimal amount, string paymentId, CancellationToken ct = default)
    {
        var options = new PaymentIntentUpdateOptions()
        {
            Amount = (long)amount,
        };
        var intent = await _paymentIntentService.UpdateAsync(paymentId, options, cancellationToken: ct);
        return new PaymentIntentResult(intent.Id, intent.ClientSecret);
    }
}
