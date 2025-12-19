using CleanArchTemplate.Aplication.Features.Security.Policy.Models.Input;
using CleanArchTemplate.Aplication.Features.Security.Policy.Models.Output;

namespace CleanArchTemplate.Aplication.Features.Security.Policy.Models;

using Domain.Security;

public static class PolicyMapper
{
    public static Policy ToPolicy(this CreatePolicyInput input)
    {
        return new Policy
        {
            Name = input.Name
        };
    }

    public static PolicyOutput ToOutput(this Policy policy)
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