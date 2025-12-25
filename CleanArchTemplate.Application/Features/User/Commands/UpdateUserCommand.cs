using CleanArchTemplate.Aplication.Abstractions.Cqrs;
using CleanArchTemplate.Aplication.Abstractions.Cqrs.Command;
using CleanArchTemplate.Aplication.Features.Auth.Services;
using CleanArchTemplate.Infrastructure.Repositories.User;
using CleanArchTemplate.Infrastructure.Services.Auth;
using CleanArchTemplate.SharedKernel.Models.User.Input;
using CleanArchTemplate.SharedKernel.Models.User.Output;
using FluentValidation;

namespace CleanArchTemplate.Aplication.Features.User.Commands;

using Domain.Users;

public sealed record UpdateUserCommand(Guid Id, UpdateUserInput Input) : ICommand<Result<UserOutput>>;

internal sealed class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IValidator<User> userValidator,
    IPasswordHashService passwordHashService) : ICommandHandler<UpdateUserCommand, Result<UserOutput>>
{
    public async Task<Result<UserOutput>> Handle(UpdateUserCommand command, CancellationToken ct)
    {
        var pass = await passwordHashService.HashPassword(command.Input.Password);
        var user = command.Input.ToUser(command.Id, pass);
        await userValidator.ValidateAndThrowAsync(user, ct);
        var updated = await userRepository.UpdateAsync(user, ct);
        return !updated 
            ? Result<UserOutput>.Failure("User update cannot be performed", ErrorType.Conflict) 
            : Result<UserOutput>.Success(user.ToOutput(), "User updated successfully");
    }
} 