using CleanArchTemplate.Aplication.Abstractions.Cqrs;
using CleanArchTemplate.Aplication.Abstractions.Cqrs.Command;
using CleanArchTemplate.Aplication.Features.Auth.Services;
using CleanArchTemplate.Infrastructure.Repositories.User;
using CleanArchTemplate.Infrastructure.Services.Auth;
using CleanArchTemplate.SharedKernel.Models.User.Input;
using CleanArchTemplate.SharedKernel.Models.User.Output;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CleanArchTemplate.Aplication.Features.User.Commands;

using Domain.Users;

public sealed record CreateUserCommand(CreateUserInput Input) : ICommand<Result<CreateUserOutput>>;


internal sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IValidator<User> userValidator,
    IPasswordHashService passwordHashService,
    ILogger<CreateUserCommandHandler> logger) : ICommandHandler<CreateUserCommand, Result<CreateUserOutput>>
{
    public async Task<Result<CreateUserOutput>> Handle(CreateUserCommand command, CancellationToken ct)
    {
        var pass = await passwordHashService.HashPassword(command.Input.Password);
        var user = command.Input.ToUser(pass);
        await userValidator.ValidateAndThrowAsync(user, ct);
        
        if (await userRepository.GetByEmailAsync(user.Email, ct) is not null)
            return Result<CreateUserOutput>.Failure("This email is already created", ErrorType.Conflict);
        
        var userId = await userRepository.CreateAsync(user, ct);
        return Result<CreateUserOutput>.Success(new (userId, user.ToOutput()), "User created successfully");
    }
} 