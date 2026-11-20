namespace E_Commerce.Application.Common;

public class PaymentGatwaySettings
{
    public string SecretKey { get; set; } = default!;
    public string DefaultCurrency { get; set; } = default!;
}
