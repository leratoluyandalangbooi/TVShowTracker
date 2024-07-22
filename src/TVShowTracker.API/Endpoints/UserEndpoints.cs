using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TVShowTracker.API.Extensions;
using TVShowTracker.Application.Abstractions;
using TVShowTracker.Application.Abstractions.Services;
using TVShowTracker.Application.DTOs.Request;

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
        group.MapPost("/register", ([FromBody] RegisterUserDto registerDto, 
            IValidator<RegisterUserDto> validator, IUserService userService, IMapper mapper)
            => RegisterUser(registerDto, validator, userService, mapper, false));

        group.MapPost("/register-admin", ([FromBody] RegisterUserDto registerDto, 
            IValidator<RegisterUserDto> validator, IUserService userService, IMapper mapper)
            => RegisterUser(registerDto, validator, userService, mapper, true)).RequireAuthorization(policy => policy.RequireRole("Admin"));

        group.MapPost("/login", LoginUser);
        group.MapGet("/profile", GetUserProfile).RequireAuthorization();
        group.MapPut("/profile", UpdateUserProfile).RequireAuthorization();
        group.MapPut("/change-password", ChangePassword).RequireAuthorization();
        group.MapDelete("/", DeleteUser).RequireAuthorization();

        return group;
    }

    private static async Task<IResult> RegisterUser(
        [FromBody] RegisterUserDto registerDto,
        IValidator<RegisterUserDto> validator,
        IUserService userService,
        IMapper mapper, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(registerDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        try
        {
            var user = await userService.RegisterUserAsync(
                registerDto.Username,
                registerDto.Email,
                registerDto.Password,
                registerDto.PreferredName,
                isAdmin, cancellationToken);

            return Results.Ok(new { message = "User registered successfully", userId = user.Username });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
    }

    private static async Task<IResult> LoginUser(
        [FromBody] LoginDto loginDto,
        IUserService userService,
        IJwtService jwtService, CancellationToken cancellationToken = default)
    {
        var user = await userService.AuthenticateAsync(loginDto.Email, loginDto.Password, cancellationToken);

        if (user == null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(user);
    }

    private static async Task<IResult> GetUserProfile(HttpContext context, IUserService userService, CancellationToken cancellationToken = default)
    {
        var userId = context.GetUserId();
        var user = await userService.GetUserByIdAsync(userId, cancellationToken);

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
        IUserService userService, CancellationToken cancellationToken = default)
    {
        var userId = context.GetUserId();

        try
        {
            await userService.UpdateUserAsync(userId, updateDto.Email, updateDto.PreferredName, cancellationToken);
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
        IUserService userService, CancellationToken cancellationToken = default)
    {
        var userId = context.GetUserId();

        try
        {
            await userService.ChangePasswordAsync(userId, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword, cancellationToken);
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

    private static async Task<IResult> DeleteUser(HttpContext context, IUserService userService, CancellationToken cancellationToken = default)
    {
        var userId = context.GetUserId();

        try
        {
            await userService.DeleteUserAsync(userId, cancellationToken);
            return Results.Ok(new { message = "User deleted successfully" });
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound(new { message = "User not found" });
        }
    }
}
