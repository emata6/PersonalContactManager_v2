using Microsoft.EntityFrameworkCore;
using PersonalContactManager.Domain.Entities;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Infrastructure.Persistence.Repositories;

public sealed class GroupRepository : IGroupRepository
{
    private readonly AppDbContext _context;

    public GroupRepository(AppDbContext context) => _context = context;

    public async Task<List<Group>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Groups.OrderBy(g => g.Name).ToListAsync(cancellationToken);

    public async Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Groups.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

    public async Task AddAsync(Group group, CancellationToken cancellationToken = default)
        => await _context.Groups.AddAsync(group, cancellationToken);

    public void Remove(Group group) => _context.Groups.Remove(group);
}
