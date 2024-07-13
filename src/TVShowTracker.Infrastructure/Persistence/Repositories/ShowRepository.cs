namespace TVShowTracker.Infrastructure.Persistence.Repositories;

public class ShowRepository : IShowRepository
{
    private readonly ApplicationDbContext _context;

    public ShowRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TopShow>> GetTopShowsFromDatabase()
    {
        return await _context.TopShows.OrderByDescending(x => x.Popularity).ToListAsync();
    }

    public async Task<TopShow?> GetTopShowByIdAsync(int id)
    {
        return await _context.TopShows.FindAsync(id);
    }

    public async Task AddAsync(TopShow show)
    {
        await _context.TopShows.AddAsync(show);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
