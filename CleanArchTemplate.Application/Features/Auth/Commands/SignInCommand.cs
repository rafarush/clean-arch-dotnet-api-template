using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Features.Auth.Services;
using CleanArchTemplate.Infrastructure.Repositories.User;
using CleanArchTemplate.Infrastructure.Services.Auth;
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
        
        var passHashed = await passwordHashService.HashPassword(command.Input.Password);
        if (!await passwordHashService.ValidatePassword(passHashed, user.Password))
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