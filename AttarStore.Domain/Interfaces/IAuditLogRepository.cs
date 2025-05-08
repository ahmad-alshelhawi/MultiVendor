// Domain/Interfaces/IAuditLogRepository.cs
using AttarStore.Domain.Entities.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AttarStore.Domain.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<IEnumerable<AuditLog>> GetAllAsync();
        Task<IEnumerable<AuditLog>> GetByActorAsync(int actorId);
        Task AddAsync(AuditLog entry);
    }
}
