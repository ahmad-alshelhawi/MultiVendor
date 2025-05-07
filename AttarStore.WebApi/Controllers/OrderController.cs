using AttarStore.Application.Dtos.Shopping;
using AttarStore.Domain.Entities;
using AttarStore.Domain.Interfaces.Shopping;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _orders;
    private readonly IMapper _mapper;

    public OrderController(IOrderRepository orders, IMapper mapper)
    {
        _orders = orders;
        _mapper = mapper;
    }

    // GET /api/order               → list own orders
    [HttpGet]
    [Authorize(Policy = "Order.ReadOwn")]
    public async Task<IActionResult> GetAll()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var orders = await _orders.GetAllForClientAsync(userId);
        return Ok(_mapper.Map<OrderDto[]>(orders));
    }

    // POST /api/order/checkout     → turn cart into an order
    [HttpPost("checkout")]
    [Authorize(Policy = "Order.Create")]
    public async Task<IActionResult> Checkout()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var order = await _orders.CheckoutAsync(userId);
        return CreatedAtAction(nameof(GetById), new { id = order.Id },
                               _mapper.Map<OrderDto>(order));
    }

    // GET /api/order/{id}
    [HttpGet("{id}")]
    [Authorize(Policy = "Order.ReadOwn")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _orders.GetByIdAsync(id);
        if (order == null) return NotFound();
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        if (order.ClientId != userId) return Forbid();
        return Ok(_mapper.Map<OrderDto>(order));
    }
}
