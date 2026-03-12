using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Infrastructure.Repositories.User;
using CleanArchTemplate.SharedKernel.Models.User.Output;

namespace CleanArchTemplate.Application.Features.User.Commands;

public sealed record DeleteUserCommand(Guid Id) : ICommand<Result<UserOutput>>;


internal sealed class DeleteUserCommandHandler(
    IUserRepository userRepository
    ) : ICommandHandler<DeleteUserCommand, Result<UserOutput>>
{
    public async Task<Result<UserOutput>> Handle(DeleteUserCommand command, CancellationToken ct)
    {
        var user = await userRepository.GetAsync(command.Id, ct);
        if (user == null)
            return Result<UserOutput>.Failure("User not found", ErrorType.NotFound);
        var deleted = await userRepository.DeleteAsync(command.Id, ct);
        return !deleted 
            ? Result<UserOutput>.Failure("User could not be deleted", ErrorType.Conflict) 
            : Result<UserOutput>.Success(user.ToOutput(), "User deleted successfully");
    }
}