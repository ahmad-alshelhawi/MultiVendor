using AttarStore.Application.Dtos.Catalog;
using AttarStore.Domain.Entities.Catalog;
using AttarStore.Domain.Interfaces.Catalog;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttarStore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepo;
        private readonly IVariantOptionRepository _optionRepo;
        private readonly IProductVariantRepository _variantRepo;
        private readonly IMapper _mapper;

        public ProductController(
            IProductRepository productRepo,
            IVariantOptionRepository optionRepo,
            IProductVariantRepository variantRepo,
            IMapper mapper)
        {
            _productRepo = productRepo;
            _optionRepo = optionRepo;
            _variantRepo = variantRepo;
            _mapper = mapper;
        }

        // ─── Standard CRUD for Products ─────────────────────────────────────────

        [HttpGet]
        [Authorize(Policy = "Product.Read")]
        public async Task<ActionResult<ProductMapperView[]>> GetAll()
        {
            var products = await _productRepo.GetAllAsync();
            return Ok(_mapper.Map<ProductMapperView[]>(products));
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Product.Read")]
        public async Task<ActionResult<ProductMapperView>> GetById(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(_mapper.Map<ProductMapperView>(product));
        }

        [HttpPost]
        [Authorize(Policy = "Product.Create")]
        public async Task<ActionResult<ProductMapperView>> Create([FromBody] ProductMapperCreate dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = _mapper.Map<Product>(dto);
            await _productRepo.AddAsync(entity);
            var result = _mapper.Map<ProductMapperView>(entity);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Product.Update")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductMapperUpdate dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _productRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);
            await _productRepo.UpdateAsync(existing);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Product.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _productRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            await _productRepo.DeleteAsync(id);
            return NoContent();
        }


        // ─── Variant‐Attribute Metadata ─────────────────────────────────────────

        [HttpGet("variant-options")]
        [Authorize(Policy = "Product.Read")]
        public async Task<ActionResult<VariantOptionDto[]>> GetVariantOptions()
        {
            var opts = await _optionRepo.GetAllOptionsAsync();
            return Ok(_mapper.Map<VariantOptionDto[]>(opts));
        }

        [HttpGet("variant-options/{optionId}/values")]
        [Authorize(Policy = "Product.Read")]
        public async Task<ActionResult<VariantOptionValueDto[]>> GetOptionValues(int optionId)
        {
            var vals = await _optionRepo.GetValuesByOptionAsync(optionId);
            if (!vals.Any()) return NotFound();
            return Ok(_mapper.Map<VariantOptionValueDto[]>(vals));
        }


        // ─── Product Variants ────────────────────────────────────────────────────

        [HttpGet("{productId}/variants")]
        [Authorize(Policy = "Product.Read")]
        public async Task<ActionResult<ProductVariantMapperView[]>> GetVariants(int productId)
        {
            var variants = await _variantRepo.GetByProductIdAsync(productId);
            return Ok(_mapper.Map<ProductVariantMapperView[]>(variants));
        }

        [HttpPost("{productId}/variants")]
        [Authorize(Policy = "Product.Update")]
        public async Task<IActionResult> CreateVariant(
            int productId,
            [FromBody] ProductVariantCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var variant = _mapper.Map<ProductVariant>(dto);
            variant.ProductId = productId;

            await _variantRepo.AddAsync(variant);
            var view = _mapper.Map<ProductVariantMapperView>(variant);

            return CreatedAtAction(
                nameof(GetVariants),
                new { productId },
                view
            );
        }

        [HttpPut("{productId}/variants/{variantId}")]
        [Authorize(Policy = "Product.Update")]
        public async Task<IActionResult> UpdateVariant(
            int productId,
            int variantId,
            [FromBody] ProductVariantCreateDto dto)
        {
            var existing = await _variantRepo.GetByIdAsync(variantId);
            if (existing == null || existing.ProductId != productId)
                return NotFound();

            _mapper.Map(dto, existing);
            await _variantRepo.UpdateAsync(existing);

            return NoContent();
        }

        [HttpDelete("{productId}/variants/{variantId}")]
        [Authorize(Policy = "Product.Delete")]
        public async Task<IActionResult> DeleteVariant(int productId, int variantId)
        {
            var existing = await _variantRepo.GetByIdAsync(variantId);
            if (existing == null || existing.ProductId != productId)
                return NotFound();

            await _variantRepo.DeleteAsync(variantId);
            return NoContent();
        }
    }
}
