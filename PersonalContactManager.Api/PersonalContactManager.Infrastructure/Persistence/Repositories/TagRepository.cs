using Microsoft.EntityFrameworkCore;
using PersonalContactManager.Domain.Entities;
using PersonalContactManager.Domain.Repositories;

namespace PersonalContactManager.Infrastructure.Persistence.Repositories;

public sealed class TagRepository : ITagRepository
{
    private readonly AppDbContext _context;

    public TagRepository(AppDbContext context) => _context = context;

    public async Task<List<Tag>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Tags.OrderBy(t => t.Name).ToListAsync(cancellationToken);

    public async Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Tags.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => await _context.Tags.FirstOrDefaultAsync(t => t.Name == name.Trim(), cancellationToken);

    public async Task AddAsync(Tag tag, CancellationToken cancellationToken = default)
        => await _context.Tags.AddAsync(tag, cancellationToken);

    public void Remove(Tag tag) => _context.Tags.Remove(tag);
}
