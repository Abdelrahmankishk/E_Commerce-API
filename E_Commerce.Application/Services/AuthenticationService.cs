using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Identity;

namespace E_Commerce.Application.Services;

internal class AuthenticationService : IAuthenticationService
{
    private readonly IIdentityService _identityService;

    public AuthenticationService(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default)
    {
        //Get User By Email
        var userResult = await _identityService.FindUserByEmailAsync(loginDto.Email);
        if (!userResult.isSuccess)
            return Result<UserDto>.Fail(userResult.Errors);

        //Check Password
        var passwordResult = await _identityService.CheckPasswordAsync(loginDto.Email, loginDto.Password);
        if (!passwordResult.isSuccess)
            return Result<UserDto>.Fail(passwordResult.Errors);

        if (!passwordResult.data)
            return Result<UserDto>.Fail(Error.Unauthorized("Invalid email or password."));

        return Result<UserDto>.Ok(new UserDto()
        {
            Email = loginDto.Email,
            DisplayName = userResult.data.DisplayName,
            Token = "token"
        });

    }
}
