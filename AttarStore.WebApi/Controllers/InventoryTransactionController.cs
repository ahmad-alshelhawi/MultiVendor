using AttarStore.Application.Dtos.Shopping;
using AttarStore.Domain.Entities.Shopping;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttarStore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryTransactionController : ControllerBase
    {
        private readonly IInventoryTransactionRepository _txRepo;
        private readonly IMapper _mapper;

        public InventoryTransactionController(
            IInventoryTransactionRepository txRepo,
            IMapper mapper)
        {
            _txRepo = txRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Record a new inventory change (stock in/out).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Record([FromBody] InventoryTransactionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tx = _mapper.Map<InventoryTransaction>(dto);
            // associate current user
            tx.UserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            await _txRepo.AddAsync(tx);
            var result = _mapper.Map<InventoryTransactionDto>(tx);

            return CreatedAtAction(nameof(GetAll), new { }, result);
        }

        /// <summary>
        /// Get all transactions, or filter by variantId or productId.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<InventoryTransactionDto[]>> GetAll(
            [FromQuery] int? variantId,
            [FromQuery] int? productId)
        {
            IEnumerable<InventoryTransaction> txs;
            if (variantId.HasValue || productId.HasValue)
                txs = await _txRepo.GetByFilterAsync(variantId, productId);
            else
                txs = await _txRepo.GetAllAsync();

            return Ok(_mapper.Map<InventoryTransactionDto[]>(txs));
        }
    }
}
