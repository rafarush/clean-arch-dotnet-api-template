using CleanArchTemplate.Application.Features.User;
using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Infrastructure.Repositories.User;
using CleanArchTemplate.Infrastructure.Services.Auth;
using CleanArchTemplate.SharedKernel.Models.User.Input;
using CleanArchTemplate.SharedKernel.Models.User.Output;
using FluentValidation;

namespace CleanArchTemplate.Application.Features.User.Commands;

public sealed record CreateUserCommand(CreateUserInput Input) : ICommand<Result<CreateUserOutput>>;


internal sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IValidator<Domain.Users.User> userValidator,
    IPasswordHashService passwordHashService) : ICommandHandler<CreateUserCommand, Result<CreateUserOutput>>
{
    public async Task<Result<CreateUserOutput>> Handle(CreateUserCommand command, CancellationToken ct)
    {
        if (await userRepository.GetByEmailAsync(command.Input.Email, ct) is not null)
            return Result<CreateUserOutput>.Failure("This email is already in use", ErrorType.Conflict);
        
        var pass = await passwordHashService.HashPassword(command.Input.Password);
        var user = command.Input.ToUser(pass);
        await userValidator.ValidateAndThrowAsync(user, ct);
        
        var userId = await userRepository.CreateAsync(user, ct);
        return Result<CreateUserOutput>.Success(new (userId, user.ToOutput()), "User created successfully");
    }
} 