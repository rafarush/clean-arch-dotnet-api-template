using CleanArchTemplate.SharedKernel.Models.Security.Policy.Input;
using CleanArchTemplate.SharedKernel.Models.Security.Policy.Output;

namespace CleanArchTemplate.Application.Features.Security.Policy;

public static class PolicyMapper
{
    public static Domain.Security.Policy ToPolicy(this CreatePolicyInput input)
    {
        return new Domain.Security.Policy
        {
            Name = input.Name
        };
    }

    public static PolicyOutput ToOutput(this Domain.Security.Policy policy)
    {
        return new PolicyOutput
        {
            CreatedAt = policy.CreatedAt,
            Id = policy.Id,
            Name = policy.Name,
            UpdatedAt = policy.UpdatedAt
        };
    }
}