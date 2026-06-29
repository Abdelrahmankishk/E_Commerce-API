using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Basketd;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    public class BasketController : ApiBaseController
    {
        private readonly IBasketService basketService;

        public BasketController(IBasketService basketService) {
            this.basketService = basketService;
        }
        // GET BaseUrl/api/Baskets/ID
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BasketDto),StatusCodes.Status200OK)]
        public async Task<ActionResult<BasketDto>> GetBasket(string id, CancellationToken ct)
        {
            var result= await basketService.GetBasketAsync(id, ct);
            return ToActionResult(result);
        }

        // POST BaseUrl/api/Baskets 
        //Body: [BasketDto]

        [HttpPost]
        public async Task<ActionResult<BasketDto>> CreateOrUpdateBasket (BasketDto basket, CancellationToken ct)
        {
            var result = await basketService.CreateOrUpdateBasketAsync(basket, ct: ct);
            return ToActionResult(result);
        }

        // DELETEBaseUrl/api/Baskets/ID
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteBasket(string id,CancellationToken ct)
        {
            var result = await basketService.DeleteBasketAsync(id, ct);
            return ToActionResult(result);
        }
    }
}
