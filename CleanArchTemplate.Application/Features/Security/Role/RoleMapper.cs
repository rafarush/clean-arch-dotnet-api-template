using CleanArchTemplate.Application.Features.Security.Policy;
using CleanArchTemplate.SharedKernel.Models.Security.Role.Input;
using CleanArchTemplate.SharedKernel.Models.Security.Role.Output;

namespace CleanArchTemplate.Application.Features.Security.Role;

public static class RoleMapper
{
    public static Domain.Security.Role ToRole(this CreateRoleInput input)
    {
        return new Domain.Security.Role()
        {
            Name = input.Name,
            Description = input.Description,
            Policies = []
        };
    }

    public static RoleOutput ToOutput(this Domain.Security.Role role)
    {
        return new RoleOutput
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            CreatedAt = role.CreatedAt.DateTime,
            UpdatedAt = role.UpdatedAt.DateTime,
            IsDeleted = role.IsDeleted,
        };
    }
    
    public static RoleDetailsOutput ToDetailsOutput(this Domain.Security.Role role)
    {
        var roleOutput = new RoleDetailsOutput
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            Policies = role.Policies.Count > 0 ? role.Policies.Select(x=> x.ToOutput()).ToList() : [],
            CreatedAt = role.CreatedAt.DateTime,
            UpdatedAt = role.UpdatedAt.DateTime,
            IsDeleted = role.IsDeleted,
        };
        return roleOutput;
    }
}