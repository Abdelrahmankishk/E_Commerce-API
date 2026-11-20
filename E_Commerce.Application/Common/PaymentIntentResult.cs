namespace E_Commerce.Application.Common;

public sealed class PaymentIntentResult
{
    public PaymentIntentResult(string paymentIntntId, string clientSecret)
    {
        PaymentIntntId = paymentIntntId;
        ClientSecret = clientSecret;
    }

    public string PaymentIntntId { get; set; } = default!;
    public string  ClientSecret { get; set; } = default!;
}
