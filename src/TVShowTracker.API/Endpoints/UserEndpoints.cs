using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TVShowTracker.API.Extensions;
using TVShowTracker.Application.Abstractions;
using TVShowTracker.Application.DTOs.User;
using TVShowTracker.Domain.Entities;

namespace TVShowTracker.API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGroup("/api/users")
            .WithTags("Users")
            .MapUserEndpoints();
    }

    private static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/register", RegisterUser);
        group.MapPost("/login", LoginUser);
        group.MapGet("/profile", GetUserProfile).RequireAuthorization();
        group.MapPut("/profile", UpdateUserProfile).RequireAuthorization();
        group.MapPut("/change-password", ChangePassword).RequireAuthorization();
        group.MapDelete("/", DeleteUser).RequireAuthorization();

        return group;
    }

    private static async Task<IResult> RegisterUser(
        [FromBody] RegisterUserDto registerDto,
        IUserService userService)
    {
        try
        {
            var user = await userService.RegisterUserAsync(
                registerDto.Username,
                registerDto.Email,
                registerDto.Password,
                registerDto.PreferredName);

            return Results.Ok(new { message = "User registered successfully", userId = user.Id });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
    }

    private static async Task<IResult> LoginUser(
        [FromBody] LoginDto loginDto,
        IUserService userService,
        IConfiguration configuration)
    {
        var user = await userService.AuthenticateAsync(loginDto.Username, loginDto.Password);

        if (user == null)
        {
            return Results.Unauthorized();
        }

        var token = GenerateJwtToken(user, configuration);

        return Results.Ok(new { token });
    }

    private static async Task<IResult> GetUserProfile(HttpContext context, IUserService userService)
    {
        var userId = context.GetUserId();
        var user = await userService.GetUserByIdAsync(userId);

        if (user == null)
        {
            return Results.NotFound(new { message = "User not found" });
        }

        return Results.Ok(new
        {
            user.Id,
            Username = context.GetUsername(),
            Email = context.GetUserEmail(),
            user.PreferredName
        });
    }

    private static async Task<IResult> UpdateUserProfile(
        [FromBody] UpdateUserProfileDto updateDto,
        HttpContext context,
        IUserService userService)
    {
        var userId = context.GetUserId();

        try
        {
            await userService.UpdateUserAsync(userId, updateDto.Email, updateDto.PreferredName);
            return Results.Ok(new { message = "Profile updated successfully" });
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound(new { message = "User not found" });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
    }

    private static async Task<IResult> ChangePassword(
        [FromBody] ChangePasswordDto changePasswordDto,
        HttpContext context,
        IUserService userService)
    {
        var userId = context.GetUserId();

        try
        {
            await userService.ChangePasswordAsync(userId, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            return Results.Ok(new { message = "Password changed successfully" });
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound(new { message = "User not found" });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
    }

    private static async Task<IResult> DeleteUser(HttpContext context, IUserService userService)
    {
        var userId = context.GetUserId();

        try
        {
            await userService.DeleteUserAsync(userId);
            return Results.Ok(new { message = "User deleted successfully" });
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound(new { message = "User not found" });
        }
    }

    private static string GenerateJwtToken(User user, IConfiguration configuration)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
