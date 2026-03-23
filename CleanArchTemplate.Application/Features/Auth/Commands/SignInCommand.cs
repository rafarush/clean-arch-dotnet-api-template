using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Features.Auth.Services;
using CleanArchTemplate.Application.Repositories.User;
using CleanArchTemplate.Application.Services.Auth.PasswordHashService;
using CleanArchTemplate.SharedKernel.Models.Auth.Input;
using CleanArchTemplate.SharedKernel.Models.Auth.Output;
using FluentValidation;

namespace CleanArchTemplate.Application.Features.Auth.Commands;

public sealed record SignInCommand(SignInInput Input) : ICommand<Result<TokenOutput>>;


internal sealed class SignInCommandHandler(
    IUserRepository userRepository,
    IValidator<SignInInput> signInCommandValidator,
    IJwtService jwtService, 
    IPasswordHashService passwordHashService) : ICommandHandler<SignInCommand, Result<TokenOutput>>
{
    public async Task<Result<TokenOutput>> Handle(SignInCommand command, CancellationToken ct)
    {
        await signInCommandValidator.ValidateAndThrowAsync(command.Input, ct);
        
        var user = await userRepository.GetByEmailAsync(command.Input.Email, ct);
        
        if (user == null)
            return Result<TokenOutput>.Unauthorized("Incorrect credentials");
        
        if (!await passwordHashService.ValidatePassword(command.Input.Password, user.Password))
            return Result<TokenOutput>.Unauthorized("Incorrect credentials");
        
        var token = jwtService.CreateToken(new TokenInput
        {
            User = user.Id,
            Email = user.Email,
            RoleNames = user.Roles.Select(r => r.Name).ToList()
        });
        
        return Result<TokenOutput>.Success(token);
    }
}