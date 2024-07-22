using TVShowTracker.Application.DTOs.Entities;
using TVShowTracker.Application.DTOs.Responses;

namespace TVShowTracker.Application.Abstractions.Services;

public interface IUserService
{
    Task<UserDto> RegisterUserAsync(string username, string email, string password, string preferredName, bool isAdmin = false, CancellationToken cancellationToken = default);
    Task<LoginResponseDto> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task UpdateUserAsync(int id, string email, string preferredName, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(int id, CancellationToken cancellationToken = default);
}
