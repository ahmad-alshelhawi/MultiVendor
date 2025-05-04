using AttarStore.Application.Dtos.Shopping;
using AttarStore.Domain.Entities.Auth;
using AttarStore.Domain.Interfaces.Shopping;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AttarStore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepo;
        private readonly IMapper _mapper;

        public CartController(ICartRepository cartRepo, IMapper mapper)
        {
            _cartRepo = cartRepo;
            _mapper = mapper;
        }

        // Helper to get the current client ID from JWT
        private int GetClientId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : throw new UnauthorizedAccessException();
        }

        [HttpGet]
        [Authorize(Roles = Roles.Client)]
        public async Task<IActionResult> GetCart()
        {
            var clientId = GetClientId();
            var cart = await _cartRepo.GetByClientIdAsync(clientId);
            return Ok(_mapper.Map<CartView>(cart));
        }

        [HttpPost("items")]
        [Authorize(Roles = Roles.Client)]
        public async Task<IActionResult> AddItem([FromBody] CartItemCreate dto)
        {
            var clientId = GetClientId();
            var item = await _cartRepo.AddItemAsync(clientId, dto.ProductVariantId, dto.Quantity);
            return CreatedAtAction(nameof(GetCart), _mapper.Map<CartItemView>(item));
        }

        [HttpPut("items/{id:int}")]
        [Authorize(Roles = Roles.Client)]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] CartItemUpdate dto)
        {
            var item = await _cartRepo.UpdateItemAsync(id, dto.Quantity);
            return Ok(_mapper.Map<CartItemView>(item));
        }

        [HttpDelete("items/{id:int}")]
        [Authorize(Roles = Roles.Client)]
        public async Task<IActionResult> RemoveItem(int id)
        {
            await _cartRepo.RemoveItemAsync(id);
            return NoContent();
        }

        [HttpDelete]
        [Authorize(Roles = Roles.Client)]
        public async Task<IActionResult> ClearCart()
        {
            var clientId = GetClientId();
            await _cartRepo.ClearCartAsync(clientId);
            return NoContent();
        }
    }
}
