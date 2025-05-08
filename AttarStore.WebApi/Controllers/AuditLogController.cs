// WebApi/Controllers/AuditLogController.cs
using AttarStore.Application.Dtos;
using AttarStore.Domain.Attributes;
using AttarStore.Domain.Entities.Auth;
using AttarStore.Domain.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AttarStore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [NoAuditForRoles("Admin")]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogRepository _repo;
        private readonly IMapper _mapper;

        public AuditLogController(IAuditLogRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // GET api/auditlog
        [HttpGet]
        public async Task<ActionResult<AuditLogDto[]>> GetAll()
        {
            var entries = await _repo.GetAllAsync();
            return Ok(_mapper.Map<AuditLogDto[]>(entries));
        }

        // GET api/auditlog/user/5
        [HttpGet("user/{actorId}")]
        public async Task<ActionResult<AuditLogDto[]>> GetByUser(int actorId)
        {
            var entries = await _repo.GetByActorAsync(actorId);
            return Ok(_mapper.Map<AuditLogDto[]>(entries));
        }
    }
}
