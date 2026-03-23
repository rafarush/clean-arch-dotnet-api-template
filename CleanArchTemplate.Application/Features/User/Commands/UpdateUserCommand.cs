using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Application.Repositories.User;
using CleanArchTemplate.Infrastructure.Services.Auth.PasswordHashService;
using CleanArchTemplate.SharedKernel.Models.User.Input;
using CleanArchTemplate.SharedKernel.Models.User.Output;
using FluentValidation;

namespace CleanArchTemplate.Application.Features.User.Commands;

using Domain.Users;

public sealed record UpdateUserCommand(Guid Id, UpdateUserInput Input) : ICommand<Result<UserOutput>>;

internal sealed class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IValidator<User> userValidator) : ICommandHandler<UpdateUserCommand, Result<UserOutput>>
{
    public async Task<Result<UserOutput>> Handle(UpdateUserCommand command, CancellationToken ct)
    {
        var user = await userRepository.GetAsync(command.Id, ct);
        if (user == null)
            return Result<UserOutput>.Failure("User not found", ErrorType.NotFound);
        
        var userUpdated = command.Input.ToUserUpdate(user);
        await userValidator.ValidateAndThrowAsync(userUpdated, ct);
        var updated = await userRepository.UpdateAsync(userUpdated, ct);
        return !updated 
            ? Result<UserOutput>.Failure("User update cannot be performed", ErrorType.Conflict) 
            : Result<UserOutput>.Success(user.ToOutput(), "User updated successfully");
    }
} 