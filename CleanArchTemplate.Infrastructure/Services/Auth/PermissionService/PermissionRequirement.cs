using Microsoft.AspNetCore.Authorization;

namespace CleanArchTemplate.Infrastructure.Services.Auth.PermissionService;

public sealed class PermissionRequirement(string policyName) : IAuthorizationRequirement
{    
    public string PolicyName { get; } = policyName;
}