using Microsoft.AspNetCore.Authorization; 
using Microsoft.Extensions.Options; 

namespace CleanArchTemplate.Infrastructure.Services.Auth.PermissionService;

public sealed class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{     
    private readonly DefaultAuthorizationPolicyProvider _fallback = new(options);

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        => _fallback.GetDefaultPolicyAsync();     
		
    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        => _fallback.GetFallbackPolicyAsync();     
	
    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {       
        var policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();         
        return Task.FromResult<AuthorizationPolicy?>(policy);    
    } 
}
