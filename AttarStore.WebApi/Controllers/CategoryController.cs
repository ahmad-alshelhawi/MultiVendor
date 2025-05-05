using System.Threading.Tasks;
using AttarStore.Application.Dtos.Catalog;
using AttarStore.Domain.Entities.Catalog;
using AttarStore.Domain.Interfaces.Catalog;
using AttarStore.Services.Interfaces.Catalog;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttarStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _catRepo;
        private readonly ISubcategoryRepository _subRepo;
        private readonly IMapper _mapper;

        public CategoryController(
            ICategoryRepository catRepo,
            ISubcategoryRepository subRepo,
            IMapper mapper)
        {
            _catRepo = catRepo;
            _subRepo = subRepo;
            _mapper = mapper;
        }

        // ─── Categories ───────────────────────────────────────────────────────────

        [HttpGet]
        [Authorize(Policy = "Category.Read")]
        public async Task<IActionResult> GetAll()
        {
            var cats = await _catRepo.GetAllAsync();
            return Ok(_mapper.Map<CategoryMapperView[]>(cats));
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Category.Read")]
        public async Task<IActionResult> GetById(int id)
        {
            var cat = await _catRepo.GetByIdAsync(id);
            if (cat == null) return NotFound();
            return Ok(_mapper.Map<CategoryMapperView>(cat));
        }

        [HttpPost]
        [Authorize(Policy = "Category.Create")]
        public async Task<IActionResult> Create([FromBody] CategoryMapperCreate dto)
        {
            var cat = _mapper.Map<Category>(dto);
            await _catRepo.AddAsync(cat);
            return CreatedAtAction(nameof(GetById), new { id = cat.Id }, _mapper.Map<CategoryMapperView>(cat));
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = "Category.Update")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryMapperUpdate dto)
        {
            var cat = await _catRepo.GetByIdAsync(id);
            if (cat == null) return NotFound();

            _mapper.Map(dto, cat);
            await _catRepo.UpdateAsync(cat);
            return Ok(_mapper.Map<CategoryMapperView>(cat));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Category.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            await _catRepo.DeleteAsync(id);
            return NoContent();
        }

        // ─── Subcategories ────────────────────────────────────────────────────────

        [HttpGet("{categoryId:int}/subcategories")]
        [Authorize(Policy = "Category.Read")]
        public async Task<IActionResult> GetSubs(int categoryId)
        {
            var subs = await _subRepo.GetByCategoryIdAsync(categoryId);
            return Ok(_mapper.Map<SubcategoryMapperView[]>(subs));
        }

        [HttpGet("subcategories/{id:int}")]
        [Authorize(Policy = "Category.Read")]
        public async Task<IActionResult> GetSubById(int id)
        {
            var sub = await _subRepo.GetByIdAsync(id);
            if (sub == null) return NotFound();
            return Ok(_mapper.Map<SubcategoryMapperView>(sub));
        }

        [HttpPost("{categoryId:int}/subcategories")]
        [Authorize(Policy = "Category.Create")]
        public async Task<IActionResult> CreateSub(
     [FromRoute] int categoryId,
     [FromBody] SubcategoryMapperCreate dto)
        {
            // map the incoming DTO to your domain entity
            var sub = _mapper.Map<Subcategory>(dto);
            // explicitly set the FK from the route
            sub.CategoryId = categoryId;

            await _subRepo.AddAsync(sub);

            var view = _mapper.Map<SubcategoryMapperView>(sub);
            return CreatedAtAction(
                nameof(GetSubById),
                new { id = sub.Id },
                view
            );
        }


        [HttpPut("subcategories/{id:int}")]
        [Authorize(Policy = "Category.Update")]
        public async Task<IActionResult> UpdateSub(int id, [FromBody] SubcategoryMapperUpdate dto)
        {
            var sub = await _subRepo.GetByIdAsync(id);
            if (sub == null) return NotFound();

            _mapper.Map(dto, sub);
            await _subRepo.UpdateAsync(sub);
            return Ok(_mapper.Map<SubcategoryMapperView>(sub));
        }

        [HttpDelete("subcategories/{id:int}")]
        [Authorize(Policy = "Category.Delete")]
        public async Task<IActionResult> DeleteSub(int id)
        {
            await _subRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}
