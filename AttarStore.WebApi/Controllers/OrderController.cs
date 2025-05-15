using AttarStore.Application.Dtos.Shopping;
using AttarStore.Domain.Entities;
using AttarStore.Domain.Interfaces.Shopping;
using AttarStore.Infrastructure.Events;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _orders;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;  // ← injected mediator

    public OrderController(
        IOrderRepository orders,
        IMapper mapper,
        IMediator mediator)                // ← add IMediator here
    {
        _orders = orders;
        _mapper = mapper;
        _mediator = mediator;
    }

    // GET /api/order → list own orders
    [HttpGet]
    [Authorize(Policy = "Order.ReadOwn")]
    public async Task<IActionResult> GetAll()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var orders = await _orders.GetAllForClientAsync(userId);
        return Ok(_mapper.Map<OrderDto[]>(orders));
    }

    // POST /api/order/checkout → turn cart into an order + publish event
    [HttpPost("checkout")]
    [Authorize(Policy = "Order.Create")]
    public async Task<IActionResult> Checkout()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // 1) create the order
        var order = await _orders.CheckoutAsync(userId);

        //// 2) extract distinct vendor IDs from each item’s variant
        //var vendorIds = order.Items
        //                     .Select(i => i.ProductVariant.Product.VendorId)
        //                     .Distinct();

        // only non-null vendor IDs, unwrapped to int
        var vendorIds = order.Items
                                    .Select(i => i.ProductVariant.Product.VendorId)
                                    .Where(v => v.HasValue)
                                    .Select(v => v.Value)
                                    .Distinct();
        // 3) publish the domain event so your MediatR handlers fire
        await _mediator.Publish(new OrderPlacedEvent(
            order.Id,
            order.ClientId,
            vendorIds
        ));

        // 4) return the created order DTO
        return CreatedAtAction(
            nameof(GetById),
            new { id = order.Id },
            _mapper.Map<OrderDto>(order)
        );
    }

    // GET /api/order/{id}
    [HttpGet("{id}")]
    [Authorize(Policy = "Order.ReadOwn")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _orders.GetByIdAsync(id);
        if (order == null) return NotFound();

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        if (order.ClientId != userId) return Forbid();

        return Ok(_mapper.Map<OrderDto>(order));
    }
}
