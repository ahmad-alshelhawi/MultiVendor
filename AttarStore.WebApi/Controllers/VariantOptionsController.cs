using AttarStore.Application.Dtos.Catalog;
using AttarStore.Domain.Entities.Catalog;
using AttarStore.Domain.Interfaces.Catalog;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttarStore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VariantOptionsController : ControllerBase
    {
        private readonly IVariantOptionRepository _repo;
        private readonly IMapper _mapper;

        public VariantOptionsController(IVariantOptionRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // GET /api/variant-options
        [HttpGet]
        [Authorize(Policy = "Product.Read")]
        public async Task<IActionResult> GetAll()
        {
            var opts = await _repo.GetAllOptionsAsync();
            return Ok(_mapper.Map<VariantOptionDto[]>(opts));
        }

        // POST /api/variant-options
        [HttpPost]
        [Authorize(Policy = "Product.Create")]
        public async Task<IActionResult> Create([FromBody] VariantOptionCreateDto dto)
        {
            var opt = new VariantOption { Name = dto.Name };
            await _repo.AddOptionAsync(opt);
            return CreatedAtAction(nameof(GetAll), new { id = opt.Id }, new { opt.Id, opt.Name });
        }

        // GET /api/variant-options/{optionId}/values
        [HttpGet("{optionId}/values")]
        [Authorize(Policy = "Product.Read")]
        public async Task<IActionResult> GetValues(int optionId)
        {
            var vals = await _repo.GetValuesByOptionAsync(optionId);
            return Ok(_mapper.Map<VariantOptionValueDto[]>(vals));
        }

        // POST /api/variant-options/{optionId}/values
        [HttpPost("{optionId}/values")]
        [Authorize(Policy = "Product.Create")]
        public async Task<IActionResult> CreateValue(int optionId, [FromBody] VariantOptionValueCreateDto dto)
        {
            var v = new VariantOptionValue
            {
                VariantOptionId = optionId,
                Value = dto.Value
            };
            await _repo.AddValueAsync(v);
            return CreatedAtAction(nameof(GetValues), new { optionId }, new { v.Id, v.Value });
        }
    }

}
