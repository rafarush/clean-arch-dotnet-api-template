using CleanArchTemplate.Aplication.Abstractions.Cqrs.Command;
using CleanArchTemplate.Aplication.Features.User.Models.Input;
using CleanArchTemplate.Aplication.Features.User.Models.Output;
using CleanArchTemplate.Application.Features.User.Models;
using CleanArchTemplate.Application.Features.User.Models.Output;
using CleanArchTemplate.Infrastructure.Repositories.User;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

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
        return new CreateUserOutput(userId, user.ToOutput());
    }
} 