using AttarStore.Domain.Entities.Auth;
using AttarStore.Domain.Interfaces;
using AttarStore.Services.Data;
using Microsoft.EntityFrameworkCore;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _db;
    public AuditLogRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(AuditLog entry)
    {
        _db.AuditLogs.Add(entry);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetAllAsync()
        => await _db.AuditLogs
                    .AsNoTracking()
                    .OrderByDescending(a => a.Timestamp)
                    .ToArrayAsync();

    public async Task<IEnumerable<AuditLog>> GetByActorAsync(int actorId)
        => await _db.AuditLogs
                    .Where(a => a.ActorId == actorId)
                    .AsNoTracking()
                    .OrderByDescending(a => a.Timestamp)
                    .ToArrayAsync();
}
