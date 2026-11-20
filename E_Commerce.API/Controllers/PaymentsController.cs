using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Basketd;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers;

public class PaymentsController : ApiBaseController
{
    private readonly IPaymentService paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        this.paymentService = paymentService;
    }
    [Authorize]
    [HttpPost("{basketId}")]
    public async Task<ActionResult<BasketDto>> CreateOrUpdatePaymentIntent(string basketId,CancellationToken ct)
    {
        return ToActionResult(await paymentService.CreateOrUpdatePaymentIntentAsync(basketId, ct));
    }
}
