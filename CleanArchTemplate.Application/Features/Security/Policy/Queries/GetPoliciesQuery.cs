using CleanArchTemplate.Application.Abstractions.Cqrs;
using CleanArchTemplate.Application.Abstractions.Cqrs.Query;
using CleanArchTemplate.Infrastructure.Repositories.Security.Policy;
using CleanArchTemplate.SharedKernel.Models.Security.Policy.Output;

namespace CleanArchTemplate.Application.Features.Security.Policy.Queries;

public sealed record GetPoliciesQuery() : IQuery<Result<IEnumerable<PolicyOutput>>>;


internal class GetPoliciesQueryHandler(
    IPolicyRepository policyRepository
) : IQueryHandler<GetPoliciesQuery, Result<IEnumerable<PolicyOutput>>>
{
    public async Task<Result<IEnumerable<PolicyOutput>>> Handle(GetPoliciesQuery query, CancellationToken ct)
    {
        var policies = await policyRepository.GetAllAsync(ct);
        var output = policies.Select(r => r.ToOutput());
        return Result<IEnumerable<PolicyOutput>>.Success(output);
    }
}