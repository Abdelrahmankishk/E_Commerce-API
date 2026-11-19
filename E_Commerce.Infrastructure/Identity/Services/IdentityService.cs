using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Identity;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Infrastructure.Identity.Services;

internal class IdentityService: IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<bool>> CheckEmailExists(string email, CancellationToken ct = default)
    {
        return await _userManager.FindByEmailAsync(email) is not null;
    }

    public async Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<bool>.Fail(Error.NotFound("User Not Found", $"User with email {email} not found."));
        else
            return await _userManager.CheckPasswordAsync(user, password);

    }

    public async Task<Result<IdentityUserResult>> CreateUserAsync(RegisterDto registerDto, CancellationToken ct = default)
    {
        var user = new ApplicationUser
        {
            Email = registerDto.Email,
            UserName = registerDto.UserName,
            DisplayName = registerDto.DisplayName,
            PhoneNumber = registerDto.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
            return Result<IdentityUserResult>.Fail(errors);

        }

        return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.DisplayName, user.Email, user.UserName));
    }

    public async Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return Result<IdentityUserResult>.Fail(Error.NotFound("User Not Found", $"User with email {email} is not found."));
        else
            return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.DisplayName, user.Email, user.UserName));
    }

    public async Task<Result<IReadOnlyList<string>>> GetUserRoles(string email, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return Error.NotFound("User Not Found", $"User With Email : {email} is not found");

        var roles = await _userManager.GetRolesAsync(user);

        return roles.ToList();
    }
}
