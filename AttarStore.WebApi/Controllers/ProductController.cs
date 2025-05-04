using AttarStore.Application.Dtos.Catalog;
using AttarStore.Domain.Interfaces.Catalog;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttarStore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _prodRepo;
        private readonly IMapper _mapper;

        public ProductController(
            IProductRepository prodRepo,
            IMapper mapper)
        {
            _prodRepo = prodRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Policy = "Product.Read")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _prodRepo.GetAllAsync();
            return Ok(_mapper.Map<ProductMapperView[]>(list));
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Product.Read")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _prodRepo.GetByIdAsync(id);
            if (p == null) return NotFound();
            return Ok(_mapper.Map<ProductMapperView>(p));
        }

        [HttpPost]
        [Authorize(Policy = "Product.Create")]
        public async Task<IActionResult> Create([FromBody] ProductMapperCreate dto)
        {
            var prod = _mapper.Map<Product>(dto);
            await _prodRepo.AddAsync(prod);
            return CreatedAtAction(nameof(GetById),
                new { id = prod.Id },
                _mapper.Map<ProductMapperView>(prod));
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "Product.Update")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductMapperUpdate dto)
        {
            var prod = await _prodRepo.GetByIdAsync(id);
            if (prod == null) return NotFound();

            _mapper.Map(dto, prod);
            await _prodRepo.UpdateAsync(prod);
            return Ok(_mapper.Map<ProductMapperView>(prod));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Product.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            await _prodRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}
