using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Infrastructure.Repositories.Security.Policy;
using CleanArchTemplate.SharedKernel.Models.Security.Policy.Output;
using Microsoft.AspNetCore.Http;

namespace CleanArchTemplate.Application.Features.Security.Policy.Commands;

public sealed record DeletePolicyCommand(Guid Id) : ICommand<Result<PolicyOutput>>;


internal sealed class DeletePolicyCommandHandler(
    IPolicyRepository policyRepository
) : ICommandHandler<DeletePolicyCommand, Result<PolicyOutput>>
{
    public async Task<Result<PolicyOutput>> Handle(DeletePolicyCommand command, CancellationToken ct)
    {
        var policy = await policyRepository.GetAsync(command.Id, ct);
        if (policy == null)
            return Result<PolicyOutput>.Failure("Policy not found", ErrorType.NotFound);
        var deleted = await policyRepository.DeleteAsync(command.Id, ct);
        return !deleted 
            ? Result<PolicyOutput>.Failure("Policy could not be deleted", ErrorType.Conflict) 
            : Result<PolicyOutput>.Success(policy.ToOutput(), "Policy deleted successfully", StatusCodes.Status204NoContent);
    }
}