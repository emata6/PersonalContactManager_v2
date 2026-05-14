using PersonalContactManager.Domain.Entities;

namespace PersonalContactManager.Domain.Repositories;

public interface IGroupRepository
{
    Task<List<Group>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Group group, CancellationToken cancellationToken = default);
    void Remove(Group group);
}
