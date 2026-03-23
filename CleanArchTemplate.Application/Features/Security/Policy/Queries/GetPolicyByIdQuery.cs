using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Query;
using CleanArchTemplate.Application.Repositories.Security.Policy;
using CleanArchTemplate.SharedKernel.Models.Security.Policy.Output;

namespace CleanArchTemplate.Application.Features.Security.Policy.Queries;

public sealed record GetPolicyByIdQuery(Guid Id) : IQuery<Result<PolicyOutput>>;


internal class GetPolicyByIdQueryHandler(
    IPolicyRepository policyRepository
) : IQueryHandler<GetPolicyByIdQuery, Result<PolicyOutput>>
{
    public async Task<Result<PolicyOutput>> Handle(GetPolicyByIdQuery query, CancellationToken ct)
    {
        var policy = await policyRepository.GetAsync(query.Id, ct);
        return policy is null ?
            Result<PolicyOutput>.Failure("Policy not found", ErrorType.NotFound) :
            Result<PolicyOutput>.Success(policy.ToOutput());
    }
}