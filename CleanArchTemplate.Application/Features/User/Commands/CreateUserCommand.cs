using CleanArchTemplate.Aplication.Abstractions.Cqrs.Command;
using CleanArchTemplate.Infrastructure.Repositories.User;
using CleanArchTemplate.SharedKernel.Models.Input.User.Models;
using CleanArchTemplate.SharedKernel.Models.Input.User.Models.Output;
using CleanArchTemplate.SharedKernel.Models.User.Input;
using FluentValidation;
// using Microsoft.AspNetCore.Identity;

namespace CleanArchTemplate.Aplication.Features.User.Commands;

using Domain.Users;

public sealed record CreateUserCommand(CreateUserInput Input) : ICommand<CreateUserOutput>;


internal sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IValidator<User> userValidator) : ICommandHandler<CreateUserCommand, CreateUserOutput>
{
    public async Task<CreateUserOutput> Handle(CreateUserCommand command, CancellationToken ct)
    {
        var user = command.Input.ToUser();
        await userValidator.ValidateAndThrowAsync(user, ct);
        var userId = await userRepository.CreateAsync(user, ct);
        return new CreateUserOutput(userId, UserMappers.ToOutput(user));
    }
} 