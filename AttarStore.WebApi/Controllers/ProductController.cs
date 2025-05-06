// WebApi/Controllers/ProductController.cs
using AttarStore.Application.Dtos.Catalog;
using AttarStore.Domain.Entities.Catalog;
using AttarStore.Domain.Interfaces.Catalog;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace AttarStore.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepo;
        private readonly IVariantOptionRepository _optRepo;
        private readonly IProductVariantRepository _varRepo;
        private readonly IMapper _mapper;

        public ProductController(
            IProductRepository productRepo,
            IVariantOptionRepository optRepo,
            IProductVariantRepository varRepo,
            IMapper mapper)
        {
            _productRepo = productRepo;
            _optRepo = optRepo;
            _varRepo = varRepo;
            _mapper = mapper;
        }

        // ─── Get all products ─────────────────────────────────────────────────────
        [HttpGet]
        [Authorize(Policy = "Product.Read")]
        public async Task<ActionResult<ProductMapperView[]>> GetAll()
        {
            var prods = await _productRepo.GetAllAsync();
            return Ok(_mapper.Map<ProductMapperView[]>(prods));
        }

        // ─── Get a single product by id ───────────────────────────────────────────
        [HttpGet("{id}")]
        [Authorize(Policy = "Product.Read")]
        public async Task<ActionResult<ProductMapperView>> GetById(int id)
        {
            var p = await _productRepo.GetByIdAsync(id);
            if (p == null) return NotFound();
            return Ok(_mapper.Map<ProductMapperView>(p));
        }

        // ─── Create a new product ─────────────────────────────────────────────────
        [HttpPost]
        [Authorize(Policy = "Product.Create")]
        public async Task<IActionResult> Create([FromBody] ProductMapperCreate dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = _mapper.Map<Product>(dto);
            await _productRepo.AddAsync(entity);

            var created = await _productRepo.GetByIdAsync(entity.Id);
            return CreatedAtAction(nameof(GetById),
                                   new { id = entity.Id },
                                   _mapper.Map<ProductMapperView>(created!));
        }

        // ─── Update an existing product ────────────────────────────────────────────
        [HttpPut("{id}")]
        [Authorize(Policy = "Product.Update")]
        public async Task<IActionResult> Update(
            int id, [FromBody] ProductMapperUpdate dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _productRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);
            await _productRepo.UpdateAsync(existing);
            return NoContent();
        }

        // ─── Delete a product ─────────────────────────────────────────────────────
        [HttpDelete("{id}")]
        [Authorize(Policy = "Product.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _productRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _productRepo.DeleteAsync(id);
            return NoContent();
        }

        // ─── Variant-option metadata ──────────────────────────────────────────────
        [HttpGet("variant-options")]
        [Authorize(Policy = "Product.Read")]
        public async Task<ActionResult<VariantOptionDto[]>> GetOptions()
        {
            var opts = await _optRepo.GetAllOptionsAsync();
            return Ok(_mapper.Map<VariantOptionDto[]>(opts));
        }

        [HttpGet("variant-options/{optId}/values")]
        [Authorize(Policy = "Product.Read")]
        public async Task<ActionResult<VariantOptionValueDto[]>> GetValues(int optId)
        {
            var vals = await _optRepo.GetValuesByOptionAsync(optId);
            if (!vals.Any()) return NotFound();
            return Ok(_mapper.Map<VariantOptionValueDto[]>(vals));
        }

        // ─── List variants for a product ──────────────────────────────────────────
        [HttpGet("{pid}/variants")]
        [Authorize(Policy = "Product.Read")]
        public async Task<ActionResult<ProductVariantMapperView[]>> GetVariants(int pid)
        {
            var p = await _productRepo.GetByIdAsync(pid);
            if (p == null) return NotFound();
            return Ok(_mapper.Map<ProductVariantMapperView[]>(p.Variants));
        }
    }
}
