using AttarStore.Application.Dtos.Shopping;
using AttarStore.Domain.Entities.Shopping;
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
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IMapper _mapper;

        public OrderController(IOrderRepository orderRepo, IMapper mapper)
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
        }

        private int GetClientId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : throw new UnauthorizedAccessException();
        }

        // ─── Create / Checkout ───────────────────────────────────────────────────
        [HttpPost]
        [Authorize(Policy = "Order.Create")]
        public async Task<IActionResult> Create([FromBody] OrderCreate dto)
        {
            var clientId = GetClientId();
            var order = _mapper.Map<Order>(dto);
            order.ClientId = clientId;

            var created = await _orderRepo.CreateAsync(order);
            return CreatedAtAction(nameof(GetById),
                new { id = created.Id },
                _mapper.Map<OrderView>(created));
        }

        // ─── Admin: Get All Orders ───────────────────────────────────────────────
        [HttpGet]
        [Authorize(Policy = "Order.ReadAll")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _orderRepo.GetAllAsync();
            return Ok(_mapper.Map<OrderView[]>(list));
        }

        // ─── Client: Get Own Orders ──────────────────────────────────────────────
        [HttpGet("own")]
        [Authorize(Policy = "Order.ReadOwn")]
        public async Task<IActionResult> GetOwn()
        {
            var clientId = GetClientId();
            var list = await _orderRepo.GetByClientAsync(clientId);
            return Ok(_mapper.Map<OrderView[]>(list));
        }

        // ─── Get By ID (Admin only) ──────────────────────────────────────────────
        [HttpGet("{id:int}")]
        [Authorize(Policy = "Order.ReadAll")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(_mapper.Map<OrderView>(order));
        }

        // ─── Update Status ───────────────────────────────────────────────────────
        [HttpPut("{id:int}/status")]
        [Authorize(Policy = "Order.Update")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderStatusUpdate dto)
        {
            var updated = await _orderRepo.UpdateStatusAsync(id, dto.Status);
            return Ok(_mapper.Map<OrderView>(updated));
        }

        // ─── Delete (Cancel) ─────────────────────────────────────────────────────
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Order.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            await _orderRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}
