namespace TVShowTracker.Application.Abstractions;

public interface IUserService
{
    Task<User> RegisterUserAsync(string username, string email, string password, string preferredName);
    Task<User> AuthenticateAsync(string username, string password);
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByUsernameAsync(string username);
    Task UpdateUserAsync(int id, string email, string preferredName);
    Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task DeleteUserAsync(int id);
}
