using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Command;
using CleanArchTemplate.Infrastructure.Repositories.Security.Policy;
using CleanArchTemplate.SharedKernel.Models.Security.Policy.Input;
using CleanArchTemplate.SharedKernel.Models.Security.Policy.Output;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace CleanArchTemplate.Application.Features.Security.Policy.Commands;

public sealed record CreatePolicyCommand(CreatePolicyInput Input) : ICommand<Result<CreatePolicyOutput>>;

internal sealed class CreatePolicyCommandHandler(
    IPolicyRepository policyRepository,
    IValidator<Domain.Security.Policy> policyValidator
    ) : ICommandHandler<CreatePolicyCommand, Result<CreatePolicyOutput>>
{
    public async Task<Result<CreatePolicyOutput>> Handle(CreatePolicyCommand command, CancellationToken ct)
    {
        var exists = await policyRepository.ExistsByNameAsync(command.Input.Name, ct);
        if (exists)
            return Result<CreatePolicyOutput>.Failure("Policy already exists", ErrorType.Conflict);
        var policy = command.Input.ToPolicy();
        await policyValidator.ValidateAndThrowAsync(policy, ct);
        var id = await policyRepository.CreateAsync(policy, ct);
        
        return id != Guid.Empty
            ? Result<CreatePolicyOutput>.Success(new(id, policy.ToOutput()), StatusCodes.Status201Created)
            : Result<CreatePolicyOutput>.Failure("Policy could not be created", ErrorType.Conflict);
    }
}
