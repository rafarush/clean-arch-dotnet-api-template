using Microsoft.AspNetCore.Authorization;

namespace CleanArchTemplate.Application.Services.Auth.PermissionService;

public sealed class PermissionRequirement(string policyName) : IAuthorizationRequirement
{    
    public string PolicyName { get; } = policyName;
}