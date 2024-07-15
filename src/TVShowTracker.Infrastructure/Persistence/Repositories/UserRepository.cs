using Microsoft.Extensions.Logging;
using System;

namespace TVShowTracker.Infrastructure.Persistence.Repositories;

internal class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Users.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting user by ID {id}");
            throw;
        }
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        try
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting user by username {username}");
            throw;
        }
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        try
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting user by email {email}");
            throw;
        }
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        try
        {
            return await _context.Users.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all users");
            throw;
        }
    }

    public async Task CreateAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        try
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while creating user {user.Username}");
            throw;
        }
    }

    public async Task UpdateAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        try
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating user {user.Id}");
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while deleting user {id}");
            throw;
        }
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        try
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while checking if username exists {username}");
            throw;
        }
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        try
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while checking if email exists {email}");
            throw;
        }
    }
}
