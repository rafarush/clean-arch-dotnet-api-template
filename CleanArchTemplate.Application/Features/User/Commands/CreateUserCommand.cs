using CleanArchTemplate.Aplication.Abstractions.Cqrs;
using CleanArchTemplate.Aplication.Abstractions.Cqrs.Command;
using CleanArchTemplate.Infrastructure.Repositories.User;
using CleanArchTemplate.SharedKernel.Models.User.Input;
using CleanArchTemplate.SharedKernel.Models.User.Output;
using FluentValidation;

namespace CleanArchTemplate.Aplication.Features.User.Commands;

using Domain.Users;

public sealed record CreateUserCommand(CreateUserInput Input) : ICommand<Result<CreateUserOutput>>;


internal sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IValidator<User> userValidator) : ICommandHandler<CreateUserCommand, Result<CreateUserOutput>>
{
    public async Task<Result<CreateUserOutput>> Handle(CreateUserCommand command, CancellationToken ct)
    {
        var user = command.Input.ToUser();
        await userValidator.ValidateAndThrowAsync(user, ct);
        var userId = await userRepository.CreateAsync(user, ct);
        return Result<CreateUserOutput>.Success(new (userId, user.ToOutput()), "User created successfully");
    }
} 