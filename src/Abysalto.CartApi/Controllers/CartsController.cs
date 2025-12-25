using AbySalto.CartApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AbySalto.CartApi.Controllers;

[ApiController]
[Route("api/carts")]
[Authorize]
public class CartsController : ControllerBase
{
    private readonly CartService _service;

    public CartsController(CartService service) => _service = service;

    private string GetUserId()
        => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "dev-user";

    [HttpPost]
    public async Task<IActionResult> CreateOrGet(CancellationToken ct)
    {
        var cart = await _service.GetOrCreateActiveCartAsync(GetUserId(), ct);
        return Ok(cart);
    }

    [HttpGet("{cartId:guid}")]
    public async Task<IActionResult> Get(Guid cartId, CancellationToken ct)
    {
        var cart = await _service.GetCartAsync(cartId, GetUserId(), ct);
        return Ok(cart);
    }

    public record AddItemRequest(string Sku, int Quantity);

    [HttpPost("{cartId:guid}/items")]
    public async Task<IActionResult> AddItem(Guid cartId, [FromBody] AddItemRequest req, CancellationToken ct)
    {
        var cart = await _service.AddItemAsync(cartId, GetUserId(), req.Sku, req.Quantity, ct);
        return Ok(cart);
    }
}
    